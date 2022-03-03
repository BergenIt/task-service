namespace TaskService.Core.SystemImplimentationType.Models;

public record SystemJobYamlSchema(
    TimeSpan Interval,
    object JsonData,
    BaseSenderSchema Sender,
    BaseSelectorSchema? Selector,
    BaseValidatorSchema? Validator,
    object Schema
) : JobYamlSchema(Sender, Selector, Validator, Schema);
