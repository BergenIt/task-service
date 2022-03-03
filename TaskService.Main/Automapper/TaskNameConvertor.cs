
using AutoMapper;

using DatabaseExtension.Translator;

using Quartz;

namespace TaskService.Automapper;

internal class TaskNameConvertor : IValueConverter<string, string>
{
    private readonly ITranslator _translator;

    public TaskNameConvertor(ITranslator translator)
    {
        _translator = translator;
    }

    public string Convert(string sourceMember, ResolutionContext context)
    {
        return _translator.GetUserText<IJob>(sourceMember);
    }
}
