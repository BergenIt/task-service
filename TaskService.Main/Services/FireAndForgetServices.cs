using AutoMapper;

using Grpc.Core;

using TaskService.Core.TaskManagers.Commands.FireAndForgetManager;
using TaskService.Core.TaskManagers.FireAndForgetManager;
using TaskService.Proto;

namespace TaskService.Services;

/// <summary>
/// Сервис запуска задач
/// </summary>
public class FireAndForgetServices : FireAndForgetService.FireAndForgetServiceBase
{
    private readonly IMapper _mapper;
    private readonly IFireAndForgetManager _fireAndForgetManager;

    /// <summary>
    /// Сервис запуска задач
    /// </summary>
    /// <param name="fireAndForgetManager"></param>
    /// <param name="mapper"></param>
    public FireAndForgetServices(IFireAndForgetManager fireAndForgetManager, IMapper mapper)
    {
        _fireAndForgetManager = fireAndForgetManager;
        _mapper = mapper;
    }

    /// <summary>
    /// Создать и запустить задачу сейчас
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<TaskKeyList> FireAndForgetTask(FireAndForgetTaskRequest request, ServerCallContext context)
    {
        IEnumerable<FireAndForgetTaskCommand> commands = _mapper.Map<IEnumerable<FireAndForgetTaskCommand>>(request.FireAndForgetTasks);

        IEnumerable<Core.Models.TaskKey> taskKeys = await _fireAndForgetManager.FireAndForgetTask(commands);

        return new()
        {
            TaskKeys = { _mapper.Map<IEnumerable<TaskKeyModel>>(taskKeys) }
        };
    }

    /// <summary>
    /// Запустить существующую задачу сейчас (если существует другой триггер, то он будет отменен)
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<TaskKeyList> FireAndForgetExistTask(FireAndForgetExistTaskRequest request, ServerCallContext context)
    {
        IEnumerable<Core.Models.TaskKey> taskKeys = await _fireAndForgetManager.FireAndForgetExistTask(request.Ids);

        return new()
        {
            TaskKeys = { _mapper.Map<IEnumerable<TaskKeyModel>>(taskKeys) }
        };
    }
}
