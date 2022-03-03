
using Newtonsoft.Json;

namespace TaskService.Core.Models;

public class JobMergedDataConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(JobMergedData);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        Dictionary<string, object?> dataMap = new();
        reader.Read();

        while (reader.TokenType is not JsonToken.EndObject)
        {
            string name = reader.Value?.ToString() ?? throw new JsonReaderException();
            reader.Read();

            string value = reader.Value?.ToString() ?? throw new JsonReaderException();

            object objectValue = value.Contains('\'') || value.Contains('{') || string.IsNullOrWhiteSpace(value)
                ? value.Replace("\'", string.Empty)
                : bool.TryParse(value, out bool boolValue)
                    ? boolValue
                    : value.Contains('.')
                        ? (object)double.Parse(value)
                        : (object)long.Parse(value);

            dataMap.Add(name, objectValue);

            reader.Read();
        }

        if (dataMap.TryGetValue(nameof(JobMergedData.Data), out object? data))
        {
            return new JobMergedData(
                 (string?)data ?? throw new JsonReaderException(),
                (string?)dataMap[nameof(JobMergedData.SenderRoute)] ?? throw new JsonReaderException(),
                (string?)dataMap.SingleOrDefault(k => k.Key == nameof(JobMergedData.ValidatorRoute)).Value,
                (string?)dataMap.SingleOrDefault(k => k.Key == nameof(JobMergedData.SelectorRoute)).Value
            );
        }

        dataMap.Remove(nameof(JobMergedData.SenderRoute), out object? senderRoute);
        dataMap.Remove(nameof(JobMergedData.ValidatorRoute), out object? validatorRoute);
        dataMap.Remove(nameof(JobMergedData.SelectorRoute), out object? selectorRoute);

        string jsonData = JsonConvert.SerializeObject(dataMap);

        return new JobMergedData(jsonData, (string?)senderRoute ?? throw new JsonReaderException(), (string?)validatorRoute, (string?)selectorRoute);
    }

    //TODO: дописать
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) { }
}
