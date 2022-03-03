namespace TaskService.Core.Models.PutTemplateSettings;

public class PutTemplateV2ForAllData
{
    [Newtonsoft.Json.JsonProperty("index_patterns")]
    public string[] IndexPatterns { get; set; } = Array.Empty<string>();

    [Newtonsoft.Json.JsonProperty("data_stream")]
    public object? DataStream { get; set; }

    [Newtonsoft.Json.JsonProperty("template")]
    public PutTemplateData? Template { get; set; }
};
