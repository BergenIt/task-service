
using DatabaseExtension;
using DatabaseExtension.Pagination;
using DatabaseExtension.Search;
using DatabaseExtension.Sort;
using DatabaseExtension.TimeRange;

using Quartz;
using Quartz.Impl.Matchers;

using TaskService.Core.Elasticsearch.Interfaces;
using TaskService.Core.Models;
using TaskService.Core.TaskRegistry;

namespace TaskService.Core.SchedulerWorkers.ScheduleGetter;

public class ScheduleGetter : IScheduleGetter
{
    private readonly IElasticsearchGetter _elasticsearchGetter;

    private readonly IScheduler _scheduler;
    private readonly ITaskRegistry _taskRegistry;
    public ScheduleGetter(ISchedulerFactory scheduler, ITaskRegistry taskRegistry, IElasticsearchGetter elasticsearchGetter)
    {
        _scheduler = scheduler.GetScheduler().GetAwaiter().GetResult();
        _taskRegistry = taskRegistry;
        _elasticsearchGetter = elasticsearchGetter;
    }

    public Task<IPageItems<ErrorExecutorLog>> GetUserTaskLog(FilterContract filter)
    {
        return _elasticsearchGetter.GetEntities<ErrorExecutorLog>(filter, nameof(ExecutorLog));
    }

    public async Task<IPageItems<ActiveTrigger>> GetActiveTriggers(TaskType taskType, FilterContract filter)
    {
        List<ActiveTrigger> commonActiveTriggers = new();

        GroupMatcher<TriggerKey> groupMatcher = GroupMatcher<TriggerKey>.GroupEquals(taskType.ToString());

        IReadOnlyCollection<TriggerKey> triggerKeys = await _scheduler.GetTriggerKeys(groupMatcher);

        foreach (TriggerKey triggerKey in triggerKeys)
        {
            TriggerState triggerState = await _scheduler.GetTriggerState(triggerKey);

            if (triggerState is TriggerState.Complete or TriggerState.Error)
            {
                continue;
            }

            ISimpleTrigger? trigger = (ISimpleTrigger?)await _scheduler.GetTrigger(triggerKey);

            if (trigger is null)
            {
                continue;
            }

            IJobDetail? detail = await _scheduler.GetJobDetail(trigger.JobKey);

            if (detail is null)
            {
                continue;
            }

            IDictionary<string, string> taskDataMap = trigger.JobDataMap.ToDictionary(
                d => d.Key,
                d => d.Value.ToString() ?? string.Empty
            );

            foreach (KeyValuePair<string, object> jobData in detail.JobDataMap)
            {
                _ = taskDataMap.TryAdd(jobData.Key, jobData.Value.ToString() ?? string.Empty);
            }

            ActiveTrigger commonActiveTrigger = new()
            {
                Id = trigger.Key.Name,
                Interval = trigger.RepeatInterval,
                JobType = trigger.JobKey.Name,
                Repeated = (uint?)trigger.RepeatCount,
                StartAt = trigger.StartTimeUtc.UtcDateTime,
                Data = taskDataMap,
                NextStart = trigger.GetNextFireTimeUtc()?.UtcDateTime,
                LastStart = trigger.GetPreviousFireTimeUtc()?.UtcDateTime,
                IsPaused = triggerState is TriggerState.Paused,
                TaskKey = trigger.Key
            };

            commonActiveTriggers.Add(commonActiveTrigger);
        }

        IEnumerable<ActiveTrigger> triggers = commonActiveTriggers
            .Search(filter.SearchFilters)
            .FilterByTimeRange(filter.TimeFilter);

        IEnumerable<ActiveTrigger> items = triggers
            .Sort(filter.SortFilters)
            .Paginations(filter.PaginationFilter);

        return new PageItems<ActiveTrigger>(items, triggers.Count());
    }

    public async Task<IPageItems<JobDescriptor>> GetJobDescriptors(TaskType taskType, FilterContract filter)
    {
        GroupMatcher<JobKey> groupMatcher = GroupMatcher<JobKey>.GroupEquals(taskType.ToString());

        IEnumerable<JobKey> jobKeys = await _scheduler.GetJobKeys(groupMatcher);

        jobKeys = jobKeys.Search(filter.SearchFilters
            .Where(s =>
                s.ColumnName.Equals(nameof(JobDescriptor.Name), StringComparison.InvariantCultureIgnoreCase)
            )
        );

        List<JobDescriptor> jobDescriptors = new();

        foreach (JobKey jobKey in jobKeys)
        {
            IJobDetail? detail = await _scheduler.GetJobDetail(jobKey);

            if (detail is null)
            {
                throw new KeyNotFoundException();
            }

            JobDescriptor jobDescriptor = new(detail.Key.Name, detail.Description ?? string.Empty);

            jobDescriptors.Add(jobDescriptor);
        }

        int count = jobDescriptors.Count;

        IEnumerable<JobDescriptor> pageDescriptors = jobDescriptors
            .Search(filter.SearchFilters)
            .Sort(filter.SortFilters)
            .Paginations(filter.PaginationFilter);

        return new PageItems<JobDescriptor>(pageDescriptors, count);
    }
}
