using Microsoft.Extensions.DependencyInjection;

namespace TaskService.Core.TaskRegistry;

public interface IServiceTaskRegistry : ITaskRegistry
{
    IServiceCollection ServiceDescriptors { get; }
}
