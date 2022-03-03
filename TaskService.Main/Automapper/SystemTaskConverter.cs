using AutoMapper;

using TaskService.Core.TaskManagers.SystemTaskManager.Commands;
using TaskService.Proto;

namespace TaskService.Automapper;

internal class SystemTaskConverter : Profile
{
    public SystemTaskConverter()
    {
        CreateMap<SystemTaskModel, Core.Models.SystemTask>()
            .ForMemberRetranslate(d => d.Name, s => s.Name);

        CreateMap<Core.Models.SystemTask, SystemTaskModel>()
            .ForMemberTranslate(d => d.Name, s => s.Name);

        CreateMap<PauseSystemTaskRequest, IEnumerable<ChangeSystemTaskPauseStatusCommand>>()
            .ConvertUsing(s => s.Ids.Select(id =>
                    new ChangeSystemTaskPauseStatusCommand(id, true)
                )
            );

        CreateMap<UnpauseSystemTaskRequest, IEnumerable<ChangeSystemTaskPauseStatusCommand>>()
            .ConvertUsing(s => s.Ids.Select(id =>
                    new ChangeSystemTaskPauseStatusCommand(id, false)
                )
            );

        CreateMap<ChangeSystemTaskIntervalModel, ChangeSystemTaskIntervalCommand>()
            .ConvertUsing(s => new ChangeSystemTaskIntervalCommand(s.Id, s.Interval.ToTimeSpan())
        );
    }
}
