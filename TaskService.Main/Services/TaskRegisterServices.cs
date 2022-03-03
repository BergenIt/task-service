using AutoMapper;

using DatabaseExtension;
using DatabaseExtension.Pagination;

using Grpc.Core;

using TaskService.Core.Models;
using TaskService.Core.SchedulerWorkers.ScheduleGetter;
using TaskService.Proto;

namespace TaskService.Services;

/// <summary>
/// Журналы выполненных и запланированных тасок 
/// </summary>
public class TaskRegisterServices : TaskRegisterService.TaskRegisterServiceBase
{
    private readonly IMapper _mapper;
    private readonly IScheduleGetter _scheduleGetter;

    /// <summary>
    /// Журналы выполненных и запланированных тасок 
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="scheduleGetter"></param>
    public TaskRegisterServices(IMapper mapper, IScheduleGetter scheduleGetter)
    {
        _mapper = mapper;
        _scheduleGetter = scheduleGetter;
    }

    /// <summary>
    /// Получить пользовательские запланированные задачи
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<ScheduleUserTaskList> GetScheduleUserTasks(GetScheduleUserTaskRequest request, ServerCallContext context)
    {
        FilterContract filter = request.Filter.FromProtoFilter<ScheduleUserTask, ActiveTrigger>();

        IPageItems<ActiveTrigger> triggers = await _scheduleGetter.GetActiveTriggers(TaskType.UserTask, filter);

        return new()
        {
            ScheduleUserTasks = { _mapper.Map<IEnumerable<ScheduleUserTask>>(triggers) },
            CountItems = triggers.CountItems,
        };
    }

    /// <summary>
    /// Получить пользовательские выполненные задачи
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<ExecutedUserTaskList> GetExecutedUserTasks(GetExecutedUserTaskRequest request, ServerCallContext context)
    {
        FilterContract filter = request.Filter.FromProtoFilter<ExecutedUserTask, ErrorExecutorLog>();

        IPageItems<ErrorExecutorLog> triggers = await _scheduleGetter.GetUserTaskLog(filter);

        return new()
        {
            ExecutedUserTasks = { _mapper.Map<IEnumerable<ExecutedUserTask>>(triggers) },
            CountItems = triggers.CountItems
        };
    }
}
