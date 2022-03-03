using AutoMapper;

using DatabaseExtension;
using DatabaseExtension.Pagination;

using Grpc.Core;

using TaskService.Core.Models;
using TaskService.Core.SchedulerWorkers.ScheduleGetter;
using TaskService.Proto;

namespace TaskService.Services;

/// <summary>
/// Получение списка типов задач и схем их данных
/// </summary>
public class TaskDescriptorServices : TaskDescriptorService.TaskDescriptorServiceBase
{
    private readonly IScheduleGetter _scheduleGetter;
    private readonly IMapper _mapper;

    /// <summary>
    /// Получение списка типов задач и схем их данных
    /// </summary>
    /// <param name="scheduleGetter"></param>
    /// <param name="mapper"></param>
    public TaskDescriptorServices(IScheduleGetter scheduleGetter, IMapper mapper)
    {
        _scheduleGetter = scheduleGetter;
        _mapper = mapper;
    }

    /// <summary>
    /// Получение списка типов задач и схем их данных
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<TaskDescriptors> GetTaskDescriptors(GetTaskDescriptorsRequest request, ServerCallContext context)
    {
        FilterContract filter = request.Filter.FromProtoFilter<TaskDescriptor, JobDescriptor>();

        int taskType = (int)request.TaskGroup;

        IPageItems<JobDescriptor> descriptors = await _scheduleGetter.GetJobDescriptors((TaskType)taskType, filter);

        return new()
        {
            Count = (int)descriptors.CountItems,
            TaskDescriptorList = { _mapper.Map<IEnumerable<TaskDescriptor>>(descriptors.Items) }
        };
    }
}
