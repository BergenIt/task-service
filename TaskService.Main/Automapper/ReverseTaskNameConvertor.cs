
using AutoMapper;

using DatabaseExtension.Translator;

using Quartz;

namespace TaskService.Automapper;

internal class ReverseTaskNameConvertor : IValueConverter<string, string>
{
    private readonly ITranslator _translator;

    public ReverseTaskNameConvertor(ITranslator translator)
    {
        _translator = translator;
    }

    public string Convert(string sourceMember, ResolutionContext context)
    {
        return _translator.GetSourceElementFromUserText<IJob>(sourceMember);
    }
}
