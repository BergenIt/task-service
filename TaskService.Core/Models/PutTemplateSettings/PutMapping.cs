using Nest;

namespace TaskService.Core.Models.PutTemplateSettings;

public class PutMapping
{
#pragma warning disable CA1822
    [Newtonsoft.Json.JsonProperty("_source")]
    public PutSourceMapping Source => new();
#pragma warning restore CA1822 

    [Newtonsoft.Json.JsonProperty("properties")]
    //public IDictionary<string, PutTemplateIProperty>? Properties { get; set; }
    public IProperties? Properties { get; set; }
}
