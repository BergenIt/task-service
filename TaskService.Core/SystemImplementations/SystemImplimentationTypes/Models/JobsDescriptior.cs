namespace TaskService.Core.SystemImplimentationType.Models;

public record JobsDescriptior(
    IDictionary<string, UserJobYamlSchema>? UserJobs,
    IDictionary<string, SystemJobYamlSchema>? SystemJobs,
    IDictionary<string, ShadowJobYamlSchema>? ShadowJobs
);
