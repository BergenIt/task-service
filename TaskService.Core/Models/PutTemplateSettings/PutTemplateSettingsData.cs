namespace TaskService.Core.Models.PutTemplateSettings;

public class PutTemplateSettingsData
{
    [Newtonsoft.Json.JsonProperty("number_of_shards")]
    public int NumberOfShards { get; set; }

    [Newtonsoft.Json.JsonProperty("number_of_replicas")]
    public int NumberOfReplicas { get; set; }

    [Newtonsoft.Json.JsonProperty("index.lifecycle.name")]
    public string IndexLifecycleName { get; set; } = string.Empty;

    [Newtonsoft.Json.JsonProperty("index.lifecycle.rollover_alias")]
    public string IndexLifecycleAlias { get; set; } = string.Empty;
}
