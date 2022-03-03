namespace TaskService.Core.SystemImplimentationType.Models;

public record JobYamlSchema(
    BaseSenderSchema Sender,
    BaseSelectorSchema? Selector,
    BaseValidatorSchema? Validator,
    object Schema
);
