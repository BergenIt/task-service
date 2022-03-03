using Nest;

namespace TaskService.Core.Models.PolicyPhases;

public abstract record PolicyPhase(TimeSpan MinimumAge)
{
    public static explicit operator Phase(PolicyPhase policy)
    {
        return new()
        {
            MinimumAge = policy.MinimumAge,
        };
    }
}
