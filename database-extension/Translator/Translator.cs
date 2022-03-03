
using Newtonsoft.Json;

using YamlDotNet.Serialization;

namespace DatabaseExtension.Translator
{
    public class Translator : ITranslator
    {
        private const string VariableKey = "USER_TEXT_JSON";
        private const string Separator = " ";
        private const string YamlType = ".yaml";
        private const string DefaultPath = "TranslatorConfig/Rus.CoreEnum.yaml";

        private readonly IDictionary<string, IDictionary<string, string>> _textMetadatas = new Dictionary<string, IDictionary<string, string>>();

        /// <summary>
        /// Чтение из стандартного файла - TranslatorConfig/Rus.CoreEnum.yaml
        /// </summary>
        public Translator()
        {
            string rawPaths = Environment.GetEnvironmentVariable(VariableKey) ?? DefaultPath;

            string[] configPaths = rawPaths.Split(Separator);

            JsonLoad(configPaths).GetAwaiter().GetResult();
        }

        public Translator(IEnumerable<string> configPaths)
        {
            JsonLoad(configPaths).GetAwaiter().GetResult();
        }

        public IDictionary<TEnum, string> GetEnumText<TEnum>() where TEnum : struct, Enum
        {
            IDictionary<string, string> enumTextMetadatas = _textMetadatas[typeof(TEnum).Name];

            return Enum.GetValues<TEnum>()
                .ToDictionary(k => k,
                t => enumTextMetadatas[t.ToString()]);
        }

        public IDictionary<Enum, string> GetEnumText(Type enumType)
        {
            object? value = typeof(Translator)
                .GetMethod(nameof(GetEnumText), 1, Type.EmptyTypes)?
                .MakeGenericMethod(new Type[] { enumType })
                .Invoke(this, Type.EmptyTypes);

            //TODO: Взять за пример на рефакторинге
            if (value is not IDictionary<Enum, string> map)
            {
                throw new NotImplementedException();
            }

            return map;
        }

        public string GetUserText<TClass>(string elementName)
        {
            return _textMetadatas[typeof(TClass).Name][elementName];
        }

        public string GetUserText(string className, string elementName)
        {
            return _textMetadatas[className][elementName];
        }

        public IEnumerable<string> GetUserText(string className)
        {
            return _textMetadatas[className].Select(t => t.Value);
        }

        public IEnumerable<string> GetUserText<TClass>()
        {
            return _textMetadatas[typeof(TClass).Name].Select(t => t.Value);
        }

        public string GetSourceElementFromUserText(string className, string userText)
        {
            return _textMetadatas[className].Single(k => k.Value == userText).Key;
        }

        public string GetSourceElementFromUserText<TClass>(string userText)
        {
            return _textMetadatas[typeof(TClass).Name].Single(k => k.Value == userText).Key;
        }

        private async Task JsonLoad(IEnumerable<string> configPaths)
        {
            foreach (string configPath in configPaths)
            {
                using StreamReader reader = new(configPath);

                string json = await reader.ReadToEndAsync();

                if (configPath.Contains(YamlType))
                {
                    using StringReader stringReader = new(json);

                    IDeserializer deserializer = new DeserializerBuilder().Build();
                    object? yamlObject = deserializer.Deserialize(stringReader);

                    if (yamlObject is null)
                    {
                        throw new NotImplementedException();
                    }

                    ISerializer serializer = new SerializerBuilder()
                        .JsonCompatible()
                        .Build();

                    json = serializer.Serialize(yamlObject);
                }

                IDictionary<string, IDictionary<string, string>>? textMetadatas = JsonConvert.DeserializeObject<IDictionary<string, IDictionary<string, string>>>(json);

                if (textMetadatas is null)
                {
                    throw new InvalidOperationException($"Не удалось спарсить файл транслятора: {json}");
                }

                foreach (KeyValuePair<string, IDictionary<string, string>> textMetadata in textMetadatas)
                {
                    _textMetadatas.Add(textMetadata);
                }

                reader.Close();
            }
        }

        TEnum ITranslator.GetSourceElementFromEnumText<TEnum>(string userText)
        {
            IDictionary<string, string> enumTextMetadatas = _textMetadatas[typeof(TEnum).Name];

            return Enum.Parse<TEnum>(enumTextMetadatas.Single(t => t.Value == userText).Key);
        }

        public void AddTranslate(string className, string key, string value)
        {
            if (!_textMetadatas.TryGetValue(className, out IDictionary<string, string>? pairs))
            {
                pairs = new Dictionary<string, string>();
                _textMetadatas.Add(className, pairs);
            }

            pairs.Add(key, value);
        }

        public void AddTranslate<TClass>(string key, string value)
        {
            AddTranslate(typeof(TClass).Name, key, value);
        }

        public IDictionary<string, string> GetFullText<TClass>()
        {
            return _textMetadatas[typeof(TClass).Name];
        }
    }
}
