
using AutoMapper;

using TaskService.Core.Models;
using TaskService.Proto;

namespace TaskService.Automapper;

internal class TaskKeyConverter : Profile
{
    public TaskKeyConverter()
    {
        CreateMap<TaskKeyModel, TaskKey>().ReverseMap();
    }
}
