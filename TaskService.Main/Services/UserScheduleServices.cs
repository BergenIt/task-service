using AutoMapper;

using Grpc.Core;

using TaskService.Core.TaskManagers.Commands.UserScheduleManager;
using TaskService.Core.TaskManagers.UserScheduleManager;
using TaskService.Proto;

namespace TaskService.Services;

/// <summary>
/// Управление пользовательскими задачами
/// </summary>
public class UserScheduleServices : UserScheduleService.UserScheduleServiceBase
{
    private readonly IUserScheduleManager _userScheduleManager;
    private readonly IMapper _mapper;

    /// <summary>
    /// Управление пользовательскими задачами
    /// </summary>
    /// <param name="userScheduleManager"></param>
    /// <param name="mapper"></param>
    public UserScheduleServices(IUserScheduleManager userScheduleManager, IMapper mapper)
    {
        _userScheduleManager = userScheduleManager;
        _mapper = mapper;
    }

    /// <summary>
    /// Создать задачу
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<TaskKeyList> CreateTask(CreateTaskRequest request, ServerCallContext context)
    {
        IEnumerable<CreateTaskCommand> commands = _mapper.Map<IEnumerable<CreateTaskCommand>>(request.CreateTasks);

        IEnumerable<Core.Models.TaskKey> taskKeys = await _userScheduleManager.CreateTask(commands);

        return new()
        {
            TaskKeys = { _mapper.Map<IEnumerable<TaskKeyModel>>(taskKeys) }
        };
    }

    /// <summary>
    /// Создать повторяющуюся задачу
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<TaskKeyList> CreateRepeatedTask(CreateRepeatedTaskRequest request, ServerCallContext context)
    {
        IEnumerable<CreateTaskCommand> commands = _mapper.Map<IEnumerable<CreateTaskCommand>>(request.CreateRepeatedTasks);

        IEnumerable<Core.Models.TaskKey> taskKeys = await _userScheduleManager.CreateTask(commands);

        return new()
        {
            TaskKeys = { _mapper.Map<IEnumerable<TaskKeyModel>>(taskKeys) }
        };
    }

    /// <summary>
    /// Перепланировать задачу
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<TaskKeyList> RescheduleTask(RescheduleTaskRequest request, ServerCallContext context)
    {
        IEnumerable<RescheduleTaskCommand> commands = _mapper.Map<IEnumerable<RescheduleTaskCommand>>(request.RescheduleTasks);

        IEnumerable<Core.Models.TaskKey> taskKeys = await _userScheduleManager.RescheduleTask(commands);

        return new()
        {
            TaskKeys = { _mapper.Map<IEnumerable<TaskKeyModel>>(taskKeys) }
        };
    }

    /// <summary>
    /// Перепланировать задачу как повторяющуюся
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<TaskKeyList> RescheduleTaskAsRepeated(RescheduleTaskAsRepeatedRequest request, ServerCallContext context)
    {
        IEnumerable<RescheduleTaskCommand> commands = _mapper.Map<IEnumerable<RescheduleTaskCommand>>(request.RescheduleRepeatedTasks);

        IEnumerable<Core.Models.TaskKey> taskKeys = await _userScheduleManager.RescheduleTask(commands);

        return new()
        {
            TaskKeys = { _mapper.Map<IEnumerable<TaskKeyModel>>(taskKeys) }
        };
    }

    /// <summary>
    /// Отменить задачу (без удаления)
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<TaskKeyList> CancelTask(CancelUserTaskRequest request, ServerCallContext context)
    {
        IEnumerable<Core.Models.TaskKey> taskKeys = await _userScheduleManager.CancelTask(request.Ids);

        return new()
        {
            TaskKeys = { _mapper.Map<IEnumerable<TaskKeyModel>>(taskKeys) }
        };
    }

    /// <summary>
    /// Поставить задачу на паузу
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<TaskKeyList> PauseTask(PauseUserTaskRequest request, ServerCallContext context)
    {
        IEnumerable<ChangeTaskPauseStatusCommand> commands = _mapper.Map<IEnumerable<ChangeTaskPauseStatusCommand>>(request);

        IEnumerable<Core.Models.TaskKey> taskKeys = await _userScheduleManager.ChangeTaskPauseStatus(commands);
        return new()
        {
            TaskKeys = { _mapper.Map<IEnumerable<TaskKeyModel>>(taskKeys) }
        };
    }

    /// <summary>
    /// Снять задачу с паузы
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<TaskKeyList> UnpauseTask(UnpauseUserTaskRequest request, ServerCallContext context)
    {
        IEnumerable<ChangeTaskPauseStatusCommand> commands = _mapper.Map<IEnumerable<ChangeTaskPauseStatusCommand>>(request);

        IEnumerable<Core.Models.TaskKey> taskKeys = await _userScheduleManager.ChangeTaskPauseStatus(commands);

        return new()
        {
            TaskKeys = { _mapper.Map<IEnumerable<TaskKeyModel>>(taskKeys) }
        };
    }

    /// <summary>
    /// Изменить данные задачи для выполнения
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<TaskKeyList> ChangeTaskData(ChangeTaskDataRequest request, ServerCallContext context)
    {
        IEnumerable<ChangeTaskDataCommand> commands = _mapper.Map<IEnumerable<ChangeTaskDataCommand>>(request.ChangeTaskDatas);

        IEnumerable<Core.Models.TaskKey> taskKeys = await _userScheduleManager.ChangeTaskData(commands);

        return new()
        {
            TaskKeys = { _mapper.Map<IEnumerable<TaskKeyModel>>(taskKeys) }
        };
    }
}
