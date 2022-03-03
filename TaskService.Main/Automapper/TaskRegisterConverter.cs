using AutoMapper;

using TaskService.Core.Models;
using TaskService.Proto;

namespace TaskService.Automapper;

internal class TaskRegisterConverter : Profile
{
    public TaskRegisterConverter()
    {
        CreateMap<ActiveTrigger, ScheduleUserTask>()
            .ForMemberTranslate(d => d.JobType, s => s.JobType);

        CreateMap<ErrorExecutorLog, ExecutedUserTask>()
            .ForMemberTranslateOrSource(d => d.JobType, s => s.JobType);
    }
}
