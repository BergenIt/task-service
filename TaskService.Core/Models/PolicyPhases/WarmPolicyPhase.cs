using Nest;

namespace TaskService.Core.Models.PolicyPhases;

public record WarmPolicyPhase : PolicyPhase
{
    public WarmPolicyPhase(TimeSpan MinimumAge) : base(MinimumAge) { }

    public static explicit operator Phase(WarmPolicyPhase policy)
    {
        Phase phase = (Phase)(policy as PolicyPhase);

        phase.Actions = new LifecycleActions();

        return phase;
    }
}
