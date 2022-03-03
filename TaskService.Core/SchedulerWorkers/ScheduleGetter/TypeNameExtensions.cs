using System.Reflection;

namespace TaskService.Core.SchedulerWorkers.ScheduleGetter;

public static class TypeNameExtensions
{
    public static string GetExecutorHumanName(this Type type)
    {
        TypeInfo typeInfo = (TypeInfo)type;

        return string.Join(";", typeInfo.GenericTypeArguments.Select(t => GetHumanName(t)));
    }

    private static string GetHumanName(Type type)
    {
        string friendlyName = type.Name;

        if (type.IsGenericType)
        {
            int iBacktick = friendlyName.IndexOf('`');
            if (iBacktick > 0)
            {
                friendlyName = friendlyName.Remove(iBacktick);
            }
            friendlyName += "<";
            Type[] typeParameters = type.GetGenericArguments();
            for (int i = 0; i < typeParameters.Length; ++i)
            {
                string typeParamName = GetHumanName(typeParameters[i]);
                friendlyName += (i == 0 ? typeParamName : "," + typeParamName);
            }
            friendlyName += ">";
        }

        return friendlyName;
    }
}
