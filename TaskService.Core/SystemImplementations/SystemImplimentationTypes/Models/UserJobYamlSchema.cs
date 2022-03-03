namespace TaskService.Core.SystemImplimentationType.Models;

public record UserJobYamlSchema(
    BaseSenderSchema Sender,
    BaseSelectorSchema? Selector,
    BaseValidatorSchema? Validator,
    object Schema
) : JobYamlSchema(Sender, Selector, Validator, Schema);
