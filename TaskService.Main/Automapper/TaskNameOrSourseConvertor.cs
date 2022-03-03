
using AutoMapper;

using DatabaseExtension.Translator;

using Quartz;

namespace TaskService.Automapper;

internal class TaskNameOrSourseConvertor : IValueConverter<string, string>
{
    private readonly ITranslator _translator;

    public TaskNameOrSourseConvertor(ITranslator translator)
    {
        _translator = translator;
    }

    public string Convert(string sourceMember, ResolutionContext context)
    {
        return _translator.GetFullText<IJob>().SingleOrDefault(v => v.Key == sourceMember).Value ?? sourceMember;
    }
}
