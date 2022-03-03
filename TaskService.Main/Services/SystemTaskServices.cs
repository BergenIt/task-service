using AutoMapper;

using DatabaseExtension;
using DatabaseExtension.Pagination;

using Grpc.Core;

using TaskService.Core.TaskManagers.SystemTaskManager;
using TaskService.Core.TaskManagers.SystemTaskManager.Commands;
using TaskService.Proto;

namespace TaskService.Services;

/// <summary>
/// Управление системными задачами
/// </summary>
public class SystemTaskServices : SystemTaskService.SystemTaskServiceBase
{
    private readonly ISystemTaskManager _systemTaskManager;
    private readonly IMapper _mapper;

    /// <summary>
    /// Управление системными задачами
    /// </summary>
    /// <param name="systemTaskManager"></param>
    /// <param name="mapper"></param>
    public SystemTaskServices(ISystemTaskManager systemTaskManager, IMapper mapper)
    {
        _systemTaskManager = systemTaskManager;
        _mapper = mapper;
    }

    /// <summary>
    ///  Получить страницу системных задач
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<SystemTaskList> GetSystemTasks(GetSystemTasksRequest request, ServerCallContext context)
    {
        FilterContract filter = request.Filter.FromProtoFilter<SystemTaskModel, Core.Models.SystemTask>();

        IPageItems<Core.Models.SystemTask> systemTasks = await _systemTaskManager.GetSystemTasks(filter);

        return new()
        {
            SystemTasks = { _mapper.Map<IEnumerable<SystemTaskModel>>(systemTasks) },
            CountItems = (int)systemTasks.CountItems,
        };
    }

    /// <summary>
    /// Изменить интервал запуска системных задач
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<TaskKeyList> ChangeSystemTaskInterval(ChangeSystemTaskIntervalRequest request, ServerCallContext context)
    {
        IEnumerable<ChangeSystemTaskIntervalCommand> commands = _mapper.Map<IEnumerable<ChangeSystemTaskIntervalCommand>>(request.ChangeSystemTaskIntervals);

        IEnumerable<Core.Models.TaskKey> taskKeys = await _systemTaskManager.ChangeSystemTaskInterval(commands);

        return new()
        {
            TaskKeys = { _mapper.Map<IEnumerable<TaskKeyModel>>(taskKeys) }
        };
    }

    /// <summary>
    /// Пауза системных задач
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<TaskKeyList> PauseSystemTask(PauseSystemTaskRequest request, ServerCallContext context)
    {
        IEnumerable<ChangeSystemTaskPauseStatusCommand> commands = _mapper.Map<IEnumerable<ChangeSystemTaskPauseStatusCommand>>(request);

        IEnumerable<Core.Models.TaskKey> taskKeys = await _systemTaskManager.ChangeSystemTaskPauseStatus(commands);

        return new()
        {
            TaskKeys = { _mapper.Map<IEnumerable<TaskKeyModel>>(taskKeys) }
        };
    }

    /// <summary>
    /// Отмена паузы системных задач
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<TaskKeyList> UnpauseSystemTask(UnpauseSystemTaskRequest request, ServerCallContext context)
    {
        IEnumerable<ChangeSystemTaskPauseStatusCommand> commands = _mapper.Map<IEnumerable<ChangeSystemTaskPauseStatusCommand>>(request);

        IEnumerable<Core.Models.TaskKey> taskKeys = await _systemTaskManager.ChangeSystemTaskPauseStatus(commands);

        return new()
        {
            TaskKeys = { _mapper.Map<IEnumerable<TaskKeyModel>>(taskKeys) }
        };
    }
}

