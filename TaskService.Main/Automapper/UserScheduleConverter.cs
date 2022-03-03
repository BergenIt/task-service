using AutoMapper;

using TaskService.Core.TaskManagers.Commands.UserScheduleManager;
using TaskService.Proto;

namespace TaskService.Automapper;

internal class UserScheduleConverter : Profile
{
    public UserScheduleConverter()
    {
        CreateMap<CreateTaskModel, CreateTaskCommand>()
            .ForMember(s => s.Interval, o => o.Ignore())
            .ForMemberRetranslate(d => d.JobType, s => s.JobType);

        CreateMap<CreateRepeatedTaskModel, CreateTaskCommand>()
            .ForMemberRetranslate(d => d.JobType, s => s.JobType);

        CreateMap<RescheduleTaskModel, RescheduleTaskCommand>()
            .ConstructUsing(s =>
                new RescheduleTaskCommand(s.Id, s.StartAt.ToDateTime(), 0, null)
            );

        CreateMap<RescheduleTaskAsRepeatedModel, RescheduleTaskCommand>()
            .ConstructUsing(s =>
                new RescheduleTaskCommand(s.Id, s.StartAt.ToDateTime(), s.RepeatedCount, s.Interval.ToTimeSpan())
            );

        CreateMap<ChangeTaskDataModel, ChangeTaskDataCommand>()
            .ConstructUsing(s =>
                new ChangeTaskDataCommand(s.Id, s.Data)
            );

        CreateMap<PauseUserTaskRequest, IEnumerable<ChangeTaskPauseStatusCommand>>()
            .ConvertUsing(s => s.Ids.Select(id =>
                new ChangeTaskPauseStatusCommand(id, true))
            );

        CreateMap<UnpauseUserTaskRequest, IEnumerable<ChangeTaskPauseStatusCommand>>()
            .ConvertUsing(s => s.Ids.Select(id =>
                new ChangeTaskPauseStatusCommand(id, false))
            );
    }
}
