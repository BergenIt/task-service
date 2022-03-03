using DatabaseExtension.Config;

namespace TaskService.StartupConfigure;

/// <summary>
/// Конфигурация для поиска и сортировка
/// </summary>
public class FilterConfigurator : PageConfigurator
{
    /// <summary>
    /// Конфигурация для поиска и сортировка
    /// </summary>
    public FilterConfigurator()
    {
        Config.InitConfigValueProfile<Proto.SystemTaskModel>()
            .AddCustomValueRoute(a => a.Name);

        Config.InitConfigValueProfile<Proto.TaskDescriptor>()
            .AddCustomValueRoute(a => a.JobName);
    }
}
