using AutoMapper;

using TaskService.Proto;

namespace TaskService.Automapper;

internal class LifecycleIndexConverter : Profile
{
    public LifecycleIndexConverter()
    {
        _ = CreateMap<LifecycleIndex, Core.Models.PolicyPhases.PolicyPhases>().ReverseMap();

        _ = CreateMap<Core.Models.PolicyPhases.HotPolicyPhase, HotPolicyPhase>();
        _ = CreateMap<Core.Models.PolicyPhases.DeletePolicyPhase, DeletePolicyPhase>();
        _ = CreateMap<Core.Models.PolicyPhases.WarmPolicyPhase, WarmPolicyPhase>();

        CreateMap<DeletePolicyPhase, Core.Models.PolicyPhases.DeletePolicyPhase>()
            .ConvertUsing(s => new Core.Models.PolicyPhases.DeletePolicyPhase(s.MinimumAge.ToTimeSpan()));

        CreateMap<WarmPolicyPhase, Core.Models.PolicyPhases.WarmPolicyPhase>()
            .ConvertUsing(s => new Core.Models.PolicyPhases.WarmPolicyPhase(s.MinimumAge.ToTimeSpan()));

        CreateMap<HotPolicyPhase, Core.Models.PolicyPhases.HotPolicyPhase>()
            .ConvertUsing(s => new Core.Models.PolicyPhases.HotPolicyPhase(s.MaximumAge.ToTimeSpan(), s.MaximumSize, s.MaximumDocuments));

    }
}
