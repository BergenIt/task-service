﻿using Nest;

namespace TaskService.Core.Models.PolicyPhases;

public record DeletePolicyPhase : PolicyPhase
{
    public DeletePolicyPhase(TimeSpan MinimumAge) : base(MinimumAge) { }

    public static explicit operator Phase(DeletePolicyPhase policy)
    {
        Phase phase = (Phase)(policy as PolicyPhase);

        LifecycleActions deleteLifecycleActions = new();
        DeleteLifecycleAction deleteLifecycleAction = new();
        deleteLifecycleActions.Add(deleteLifecycleAction);

        phase.Actions = deleteLifecycleActions;

        return phase;
    }
}
