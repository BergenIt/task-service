using Nest;

namespace TaskService.Core.Models.PutTemplateSettings;

public class PutTemplateIProperty : IProperty
{
    public IDictionary<string, object>? LocalMetadata { get; set; }
    public IDictionary<string, string>? Meta { get; set; }
    public PropertyName? Name { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool? Fielddata { get; set; }
}
