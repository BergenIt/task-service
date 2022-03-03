
using Quartz;

using TaskService.Core.Models;
using TaskService.Core.SchedulerWorkers.ScheduleGetter;

namespace TaskService.Core.SchedulerWorkers.JobDetailBuilder;

public class JobDetailBuilder : IJobDetailBuilder
{
    public IJobDetail BuildJob(Type jobType, TaskType taskType, string name, string? comment = null)
    {
        name ??= jobType.GetExecutorHumanName();
        comment ??= string.Empty;

        JobDataMap dataMap = new();//CreateDataMap(data);

        IJobDetail jobDetail = JobBuilder
            .Create(jobType)
            .UsingJobData(dataMap)
            .WithDescription(comment)
            .WithIdentity(name, taskType.ToString())
            .Build();

        return jobDetail;
    }

    public ITrigger BuildTrigger(TriggerKey triggerKey, JobKey jobKey, DateTime? startAt, uint? repeatCount, TimeSpan interval, IDictionary<string, string>? data = null)
    {
        data ??= new Dictionary<string, string>();

        JobDataMap dataMap = CreateDataMap(data);

        TriggerBuilder triggerBuilder = TriggerBuilder
            .Create()
            .ForJob(jobKey)
            .UsingJobData(dataMap)
            .WithIdentity(triggerKey)
            .WithSimpleSchedule(a =>
            {
                a.WithInterval(interval);
                a.WithMisfireHandlingInstructionIgnoreMisfires();

                _ = repeatCount.HasValue
                    ? a.WithRepeatCount((int)repeatCount.Value)
                    : a.RepeatForever();
            });

        triggerBuilder = startAt.HasValue
            ? triggerBuilder.StartAt(startAt.Value)
            : triggerBuilder.StartNow();

        return triggerBuilder.Build();
    }

    private static JobDataMap CreateDataMap(IDictionary<string, string> data)
    {
        IDictionary<string, object> jsonData = data.ToDictionary(
            k => k.Key,
            v => (object)v.Value
        );

        return new(jsonData);
    }
}
