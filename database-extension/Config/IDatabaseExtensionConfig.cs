
using System.Linq.Expressions;

namespace DatabaseExtension.Config;

public interface IDatabaseExtensionConfig
{
    IConfigProfile<TS, TD> AddCustomRoute<TS, TD>(string sourseRoute, string distRoute)
        where TS : class
        where TD : class;

    IConfigProfile<TS, TD> AddCustomRoute<TS, TSP, TD, TDP>(Expression<Func<TS, TSP>> sourseRoute, Expression<Func<TD, TDP>> distRoute)
        where TS : class
        where TD : class;

    string GetDistinationName<TS, TD>(string sourceName)
        where TS : class
        where TD : class;

    string GetDistinationValue<TD>(string sourceName, string value) where TD : class;

    IConfigProfile<TS, TD> InitConfigProfile<TS, TD>()
        where TS : class
        where TD : class;

    IConfigValueProfile<TD> InitConfigValueProfile<TD>() where TD : class;
}
