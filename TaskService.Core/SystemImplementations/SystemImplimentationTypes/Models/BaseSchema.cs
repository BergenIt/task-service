namespace TaskService.Core.SystemImplimentationType.Models;

public record BaseSenderSchema(ImplimentationType Type, object Router) : BaseImplementation(Type, Router);
public record BaseSelectorSchema(ImplimentationType Type, object Router) : BaseImplementation(Type, Router);
public record BaseValidatorSchema(ImplimentationType Type, object Router) : BaseImplementation(Type, Router);
