
using Microsoft.Extensions.Configuration;

namespace TaskService.Core.Options;

public abstract class ProjectOptionsLogic
{
    private readonly IConfiguration _configuration;

    protected ProjectOptionsLogic(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IEnumerable<string> GetVariables(string configurationName, bool validationIgnore = false)
    {
        IEnumerable<string> values = _configuration.GetSection(configurationName).Get<IEnumerable<string>>();

        if (values is null)
        {
            return validationIgnore
                ? Array.Empty<string>()
                : throw new ConfigurationVariableException(configurationName);
        }

        return values;
    }

    public IEnumerable<T> GetVariables<T>(string configurationName, bool validationIgnore = false) where T : IConvertible
    {
        IEnumerable<string> values = _configuration.GetSection(configurationName).Get<IEnumerable<string>>();

        if (values is null)
        {
            return validationIgnore
                ? Array.Empty<T>()
                : throw new ConfigurationVariableException(configurationName);
        }

        IEnumerable<T> result = values.Select(v => (T)(v as IConvertible).ToType(typeof(T), null));

        return result;
    }

    public T GetVariable<T>(string configurationName) where T : struct, Enum
    {
        string value = _configuration.GetValue<string>(configurationName);

        return Enum.TryParse(value, out T enumValue)
            ? enumValue
            : throw new ConfigurationVariableException(configurationName);
    }

    public T? GetVariable<T>(string configurationName, bool validationIgnore = false) where T : IConvertible
    {
        string value = _configuration.GetValue<string>(configurationName);

        if (string.IsNullOrWhiteSpace(value))
        {
            return validationIgnore
                ? default
                : throw new ConfigurationVariableException(configurationName);
        }

        return (T?)(value as IConvertible)?.ToType(typeof(T), null);
    }

    public string GetVariable(string configurationName, bool validationIgnore = false)
    {
        string value = _configuration.GetValue<string>(configurationName);

        if (string.IsNullOrWhiteSpace(value) && !validationIgnore)
        {
            throw new ConfigurationVariableException(configurationName);
        }

        return value ?? string.Empty;
    }
}
