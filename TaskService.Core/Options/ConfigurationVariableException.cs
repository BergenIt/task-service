namespace TaskService.Core.Options;

public class ConfigurationVariableException : Exception
{
    private const string ExceptionText = "Не найдена переменная конфигурации: ";
    public ConfigurationVariableException(string variableName)
        : base($"{ExceptionText}{variableName}") { }
}
