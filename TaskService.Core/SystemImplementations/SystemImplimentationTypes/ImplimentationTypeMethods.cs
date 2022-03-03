using System.Reflection;

namespace TaskService.Core.SystemImplimentationType;

public static class ImplimentationTypeMethods
{
    public static Type GetDataType(this ImplimentationType implimentationType)
    {
        ImplimentationTypeAttribute? implimentationTypeAttribute = GetAttribute(implimentationType);

        return implimentationTypeAttribute.Datatype;
    }

    public static Type GetValidatorType(this ImplimentationType implimentationType)
    {
        ImplimentationTypeAttribute? implimentationTypeAttribute = GetAttribute(implimentationType);

        return implimentationTypeAttribute.ValidatorType;
    }

    public static Type GetSelectorType(this ImplimentationType implimentationType)
    {
        ImplimentationTypeAttribute? implimentationTypeAttribute = GetAttribute(implimentationType);

        return implimentationTypeAttribute.SelectorType;
    }

    public static Type GetSenderType(this ImplimentationType implimentationType)
    {
        ImplimentationTypeAttribute? implimentationTypeAttribute = GetAttribute(implimentationType);

        return implimentationTypeAttribute.SenderType;
    }

    private static ImplimentationTypeAttribute GetAttribute(ImplimentationType implimentationType)
    {
        ImplimentationTypeAttribute? implimentationTypeAttribute = typeof(ImplimentationType)
            .GetField(implimentationType.ToString())?
            .GetCustomAttribute<ImplimentationTypeAttribute>();

        if (implimentationTypeAttribute is null)
        {
            throw new ArgumentNullException(nameof(implimentationType));
        }

        return implimentationTypeAttribute;
    }
}
