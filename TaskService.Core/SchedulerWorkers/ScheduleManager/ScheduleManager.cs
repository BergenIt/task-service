using Quartz;

using TaskService.Core.AuditWriter;
using TaskService.Core.Models;
using TaskService.Core.SchedulerWorkers.JobDetailBuilder;

namespace TaskService.Core.SchedulerWorkers.ScheduleManager;

public class ScheduleManager : IScheduleManager
{
    private readonly IScheduler _scheduler;
    private readonly IJobDetailBuilder _jobDetailBuilder;

    private readonly IAuditWriter _auditWriter;

    public ScheduleManager(ISchedulerFactory scheduler, IJobDetailBuilder jobDetailBuilder, IAuditWriter auditWriter)
    {
        _scheduler = scheduler.GetScheduler().GetAwaiter().GetResult();
        _auditWriter = auditWriter;
        _jobDetailBuilder = jobDetailBuilder;
    }
    public async Task<TaskKey> ScheduleJobAsync(string jobName, TaskType taskType, IDictionary<string, string> data, DateTime? startAt, uint? repeatCount, TimeSpan interval)
    {
        ITrigger trigger = _jobDetailBuilder.BuildTrigger(
            new(Guid.NewGuid().ToString(), taskType.ToString()),
            new(jobName, taskType.ToString()),
            startAt,
            repeatCount,
            interval,
            data
        );

        _ = await _scheduler.ScheduleJob(trigger);

        await _auditWriter.UpsertJobRecord(AuditTaskAction.Create, taskType, jobName, data);

        return trigger.Key;
    }

    public async Task<TaskKey> CancelJobAsync(string triggerKey, TaskType taskType)
    {
        TriggerKey key = new(triggerKey, taskType.ToString());

        ITrigger? trigger = await _scheduler.GetTrigger(key);

        if (trigger is null)
        {
            throw new KeyNotFoundException();
        }

        await _scheduler.UnscheduleJob(key);

        IDictionary<string, string> taskData = trigger.JobDataMap.CreateDataMap();

        await _auditWriter.UpsertJobRecord(AuditTaskAction.Cancel, taskType, trigger.JobKey.Name, taskData);

        return trigger.Key;
    }

    public async Task<TaskKey> RescheduleJobAsync(string triggerKey, TaskType taskType, DateTime? startAt, uint? repeatCount, TimeSpan interval)
    {
        TriggerKey key = new(triggerKey, taskType.ToString());

        ITrigger? rmTrigger = await _scheduler.GetTrigger(key);

        if (rmTrigger is null)
        {
            throw new KeyNotFoundException();
        }

        IDictionary<string, string> taskData = rmTrigger.JobDataMap.CreateDataMap();

        ITrigger trigger = _jobDetailBuilder.BuildTrigger(
            rmTrigger.Key,
            rmTrigger.JobKey,
            startAt,
            repeatCount,
            interval,
            taskData
        );

        await _scheduler.RescheduleJob(key, trigger);

        await _auditWriter.UpsertJobRecord(AuditTaskAction.Rechedule, taskType, trigger.JobKey.Name, taskData);

        return trigger.Key;
    }

    public async Task<TaskKey> PauseTriggerAsync(string triggerKey, TaskType taskType)
    {
        TriggerKey key = new(triggerKey, taskType.ToString());

        ITrigger? trigger = await _scheduler.GetTrigger(key);

        if (trigger is null)
        {
            throw new KeyNotFoundException();
        }

        await _scheduler.PauseTrigger(key);

        IDictionary<string, string> taskData = trigger.JobDataMap.CreateDataMap();

        await _auditWriter.UpsertJobRecord(AuditTaskAction.Pause, taskType, trigger.JobKey.Name, taskData);

        return trigger.Key;
    }

    public async Task<TaskKey> UnPauseTriggerAsync(string triggerKey, TaskType taskType)
    {
        TriggerKey key = new(triggerKey, taskType.ToString());

        ITrigger? trigger = await _scheduler.GetTrigger(key);

        if (trigger is null)
        {
            throw new KeyNotFoundException();
        }

        await _scheduler.ResumeTrigger(key);

        IDictionary<string, string> taskData = trigger.JobDataMap.CreateDataMap();

        await _auditWriter.UpsertJobRecord(AuditTaskAction.Unpause, taskType, trigger.JobKey.Name, taskData);

        return trigger.Key;
    }

    public async Task<TaskKey> ChangeTaskData(string triggerStrKey, TaskType taskType, IDictionary<string, string> inputData)
    {
        TriggerKey triggerKey = new(triggerStrKey, taskType.ToString());

        ITrigger? trigger = await _scheduler.GetTrigger(triggerKey);

        if (trigger is null)
        {
            throw new KeyNotFoundException();
        }

        foreach (KeyValuePair<string, string> item in inputData)
        {
            _ = trigger.JobDataMap.Remove(item.Key);

            if (!string.IsNullOrEmpty(item.Value))
            {
                trigger.JobDataMap.Add(item.Key, item.Value);
            }
        }

        await _scheduler.ScheduleJob(trigger);

        IDictionary<string, string> taskData = trigger.JobDataMap.CreateDataMap();

        await _auditWriter.UpsertJobRecord(AuditTaskAction.DataChange, taskType, trigger.JobKey.Name, taskData);

        return trigger.Key;
    }
}
