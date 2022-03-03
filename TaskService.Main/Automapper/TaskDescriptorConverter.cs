using AutoMapper;

using TaskService.Core.Models;
using TaskService.Proto;

namespace TaskService.Automapper;

internal class TaskDescriptorConverter : Profile
{
    public TaskDescriptorConverter()
    {
        CreateMap<JobDescriptor, TaskDescriptor>()
            .ForMember(s => s.DataSchema, o => o.MapFrom(c => c.Schema))
            .ForMemberTranslate(s => s.JobName, s => s.Name);
    }
}
