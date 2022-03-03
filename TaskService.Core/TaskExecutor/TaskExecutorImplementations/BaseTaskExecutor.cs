using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

using Quartz;

using TaskService.Core.Elasticsearch;
using TaskService.Core.Elasticsearch.Interfaces;
using TaskService.Core.Models;
using TaskService.Core.SchedulerWorkers;

namespace TaskService.Core.TaskExecutor.TaskExecutorImplementations;

public abstract class BaseTaskExecutor : ITaskExecutor, IJob
{
    protected readonly JsonSerializerSettings _settings;

    private readonly IServiceProvider _rootServiceProvider;
    private readonly IElasticsearchWorker _elasticsearchWorker;

    private IServiceProvider? _serviceProvider;

    protected BaseTaskExecutor(IServiceProvider serviceProvider, IElasticsearchWorker elasticsearchWorker)
    {
        _rootServiceProvider = serviceProvider;
        _elasticsearchWorker = elasticsearchWorker;

        _settings = JsonConvert.DefaultSettings is not null ? JsonConvert.DefaultSettings() : (new());
    }

    protected async Task InvokeSender<TMessageSender, TMsg>(TaskMeta task, TMsg msg)
        where TMsg : class
        where TMessageSender : class, IMessageSender<TMsg>
    {
        if (_serviceProvider is null)
        {
            throw new TaskExecutorNullServiceException<TMessageSender>();
        }

        TMessageSender messageSender = _serviceProvider.GetRequiredService<TMessageSender>();

        await messageSender.Send(task.Id, task.TaskType, msg);
    }

    protected async ValueTask<TMsg> InvokeSelector<TMessageSelector, TData, TMsg>(TaskMeta task, TData taskData)
        where TData : class
        where TMsg : class
        where TMessageSelector : class, IMessageSelector<TData, TMsg>
    {
        if (_serviceProvider is null)
        {
            throw new TaskExecutorNullServiceException<TMessageSelector>();
        }

        TMessageSelector messageSelector = _serviceProvider.GetRequiredService<TMessageSelector>();

        TMsg msg = await messageSelector.SelectMessage(task.Id, task.TaskType, taskData);

        if (msg is null)
        {
            throw new SelectorException<TMessageSelector, TData, TMsg>();
        }

        return msg;
    }

    protected async ValueTask InvokeValidator<TTaskValidator, TData>(TaskMeta task, TData taskData)
        where TData : class
        where TTaskValidator : class, ITaskValidator<TData>
    {
        if (_serviceProvider is null)
        {
            throw new TaskExecutorNullServiceException<TTaskValidator>();
        }

        TTaskValidator? taskValidator = _serviceProvider.GetRequiredService<TTaskValidator>();

        bool isValid = await taskValidator.Validate(task.Id, task.TaskType, taskData);

        if (!isValid)
        {
            throw new TaskNotValidException<TTaskValidator, TData>();
        }
    }

    public abstract Task Execute(TaskMeta task);

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            if (!TryBuildTaskData(context, out TaskType taskType, out TaskMeta task, out ErrorExecutorLog executorLog))
            {
                Serilog.Log.Logger.ForContext<BaseTaskExecutor>().Fatal("BuildTaskData error {context.JobDetail}", context.JobDetail);

                return;
            }

            using IServiceScope serviceScope = _rootServiceProvider.CreateScope();
            {
                _serviceProvider = serviceScope.ServiceProvider;

                try
                {
                    await Execute(task);
                }
                catch (Exception ex)
                {
                    Serilog.Log.Logger.ForContext<BaseTaskExecutor>().Error(ex, "Task execute error {ex}");

                    if (taskType is TaskType.UserTask)
                    {
                        executorLog = new ErrorExecutorLog(
                            executorLog.Id,
                            executorLog.JobType,
                            executorLog.StartAt,
                            executorLog.Data,
                            executorLog.JobSheduler,
                            executorLog.TaskKey,
                            ex.Message
                        );
                    }
                }
            }

            if (taskType is TaskType.UserTask)
            {
                Serilog.Log.Logger.ForContext<BaseTaskExecutor>().Information("User task execute {executorLog}", executorLog);

                await _elasticsearchWorker.InsertAsync(executorLog, nameof(ExecutorLog));
            }
            else
            {
                Serilog.Log.Logger.ForContext<BaseTaskExecutor>().Debug("Task execute {executorLog}", executorLog);
            }
        }
        catch (ElasticWorkerException ex)
        {
            Serilog.Log.Logger.ForContext<BaseTaskExecutor>().Error(ex, "Task execute elastic write error {ex}");
        }
        catch (Exception ex)
        {
            Serilog.Log.Logger.ForContext<BaseTaskExecutor>().Fatal(ex, "Task executor parse {ex}");

            //throw new JobExecutionException();
        }
    }

    private static bool TryBuildTaskData(IJobExecutionContext context, out TaskType taskType, out TaskMeta meta, out ErrorExecutorLog executorLog)
    {
        TriggerKey triggerKey = context.Trigger.Key;

        bool validGroup = Enum.TryParse(triggerKey.Group, out taskType);

        if (!validGroup)
        {
            meta = new();
            executorLog = new(
                string.Empty,
                string.Empty,
                default,
                new Dictionary<string, string>(),
                new(null,
                    null,
                    null,
                    null
                ),
                new TaskKey(
                    string.Empty,
                    default
                ),
                string.Empty
            );

            return false;
        }

        ISimpleTrigger? simpleTrigger = context.Trigger as ISimpleTrigger;

        JobSheduler jobSheduler = new(
            simpleTrigger?.RepeatInterval,
            context.Trigger.GetNextFireTimeUtc(),
            context.Trigger.GetPreviousFireTimeUtc(),
            (uint?)simpleTrigger?.RepeatCount
        );

        //string jobType = context.JobDetail.JobType.GetExecutorHumanName();
        string jobType = context.JobDetail.Key.Name;

        IDictionary<string, string> data = context.MergedJobDataMap.CreateDataMap();

        meta = new()
        {
            Id = context.FireInstanceId,
            TaskType = jobType,
            MergedJson = JsonConvert.SerializeObject(context.MergedJobDataMap),
        };

        executorLog = new(
            context.FireInstanceId,
            meta.TaskType,
            DateTime.UtcNow,
            data,
            jobSheduler,
            context.Trigger.Key,
            string.Empty
        );

        return true;
    }
}
