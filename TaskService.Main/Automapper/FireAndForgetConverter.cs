
using AutoMapper;

using TaskService.Core.TaskManagers.Commands.FireAndForgetManager;
using TaskService.Proto;

namespace TaskService.Automapper;

internal class FireAndForgetConverter : Profile
{
    public FireAndForgetConverter()
    {
        CreateMap<FireAndForgetTaskModel, FireAndForgetTaskCommand>()
            .ForMemberRetranslate(d => d.JobType, s => s.JobType);
    }
}
