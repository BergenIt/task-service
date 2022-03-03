
using Quartz;
using Quartz.Impl.Matchers;

using TaskService.Core.Models;
using TaskService.Core.TaskRegistry;

namespace TaskService.Core.SchedulerWorkers.QuartzMigrator;

public class QuartzMigrator : IQuartzMigrator
{
    private readonly IScheduler _scheduler;

    public QuartzMigrator(ISchedulerFactory scheduler)
    {
        _scheduler = scheduler.GetScheduler().GetAwaiter().GetResult();
    }

    public async Task Migrate(IDictionary<JobKey, BackTaskRegistry> jobsFromStart, WriteLevel level)
    {
        GroupMatcher<JobKey> matcher = GroupMatcher<JobKey>.AnyGroup();

        IReadOnlyCollection<JobKey> existJobKeys = await _scheduler.GetJobKeys(matcher);

        foreach (KeyValuePair<JobKey, BackTaskRegistry> job in jobsFromStart)
        {
            if (level is WriteLevel.Force or WriteLevel.Update || (level is WriteLevel.Passive && !existJobKeys.Contains(job.Key)))
            {
                await AddJobToStore(job.Key, job.Value);
            }
        }

        if (level is WriteLevel.Force)
        {
            IReadOnlyCollection<JobKey> removeJobKeys = existJobKeys
                .Where(k => !jobsFromStart.ContainsKey(k))
                .ToArray();

            await _scheduler.DeleteJobs(removeJobKeys);
        }
    }

    private async Task AddJobToStore(JobKey jobKey, BackTaskRegistry job)
    {
        IJobDetail jobDetail = JobBuilder
            .Create(job.TaskType)
            .UsingJobData(CreateDataMap(job.RootData))
            .WithDescription(job.Schema)
            .WithIdentity(jobKey)
            .StoreDurably()
            .Build();

        if (job.TimeSpan == TimeSpan.Zero)
        {
            await _scheduler.AddJob(jobDetail, true);
            return;
        }

        Quartz.ITrigger trigger = TriggerBuilder
            .Create()
            .ForJob(jobKey)
            .UsingJobData(CreateDataMap(job.Data))
            .WithIdentity(jobKey.Name, jobKey.Group)
            .WithSimpleSchedule(a =>
            {
                a.WithInterval(job.TimeSpan);
                a.WithMisfireHandlingInstructionIgnoreMisfires();
                a.RepeatForever();
            })
            .StartNow()
            .Build();

        await _scheduler.ScheduleJob(jobDetail, new ITrigger[] { trigger }, true);
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
