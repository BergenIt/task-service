
using Microsoft.Extensions.DependencyInjection;

namespace TaskService.Core.TaskRegistry;

public record TaskRegistryRecord(
        IServiceCollection ServiceDescriptors,
        IDictionary<string, TaskRegistryDescriptor> UserJobs,
        IDictionary<string, BackTaskRegistry> ShadowJobs,
        IDictionary<string, BackTaskRegistry> SystemJobs
    ) : IServiceTaskRegistry;
