using System.Reflection;

using AutoMapper;

using DatabaseExtension.Group;
using DatabaseExtension.Group.PageExtensions;
using DatabaseExtension.Search;
using DatabaseExtension.Sort;
using DatabaseExtension.Translator;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DatabaseExtension.Config;

public static class PageConfiguratorBuilder
{
    public static IApplicationBuilder UsePageConfigurator(this IApplicationBuilder builder, Assembly assembly)
    {
        ITranslator translator = builder.ApplicationServices.GetRequiredService<ITranslator>();

        IMapper mapper = builder.ApplicationServices.GetRequiredService<IMapper>();

        PageExtensions.InjectMapper(mapper);
        PageExtensions.InjectAssembly(assembly);

        IEnumerable<PageConfigurator> pageConfigurators = assembly.DefinedTypes
            .Where(t => t.BaseType == typeof(PageConfigurator))
            .Select(t => (PageConfigurator?)Activator.CreateInstance(t) ?? throw new InvalidOperationException());

        DatabaseExtensionConfig extensionConfig = new(translator);

        foreach (PageConfigurator pageConfigurator in pageConfigurators)
        {
            extensionConfig._routeDictionaryList.AddRange(pageConfigurator._databaseExtensionConfig._routeDictionaryList);
            extensionConfig._routeDictionaryValueList.AddRange(pageConfigurator._databaseExtensionConfig._routeDictionaryValueList);
        }

        SortConverter.InjectConfig(extensionConfig);
        SearchConverter.InjectConfig(extensionConfig);
        GroupConverter.InjectConfig(extensionConfig);

        return builder;
    }
}
