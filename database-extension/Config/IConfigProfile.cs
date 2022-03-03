namespace DatabaseExtension.Config;

public interface IConfigProfile<TS, TD>
    where TS : class
    where TD : class
{
    public IDictionary<string, string> RouteDictionary { get; }
};

public interface IConfigValueProfile<TD>
    where TD : class
{
    public IDictionary<Type, string> RouteDictionary { get; }
};
