namespace TaskService.Core.Models.PutTemplateSettings;

public class PutTemplateData
{
    [Newtonsoft.Json.JsonProperty("settings")]
    public PutTemplateSettingsData? Settings { get; set; }

    [Newtonsoft.Json.JsonProperty("mappings")]
    public string TypeMapping { get; set; } = string.Empty;
}
