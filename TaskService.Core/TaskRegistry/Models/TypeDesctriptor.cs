using System.Diagnostics.CodeAnalysis;

namespace TaskService.Core.TaskRegistry;

public record TypeDesctriptor(
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type Type,
    Func<IServiceProvider, object>? ImplementationFactory
);
