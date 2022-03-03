
using TaskService.Core.Models;

namespace TaskService.Core.SystemImplimentationType;

[AttributeUsage(AttributeTargets.Field)]
public class ImplimentationTypeAttribute : Attribute
{
    public ImplimentationTypeAttribute(Type validatorType, Type selectorType, Type senderType)
    {
        ValidatorType = validatorType;
        SelectorType = selectorType;
        SenderType = senderType;
    }

    public Type ValidatorType { get; init; }
    public Type SelectorType { get; init; }
    public Type SenderType { get; init; }

#pragma warning disable CA1822 // Заглушка на случай если понадобятся другие типы данных
    public Type Datatype => typeof(JobMergedData);
#pragma warning restore CA1822 // Пометьте члены как статические
}
