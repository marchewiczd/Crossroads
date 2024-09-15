using System.ComponentModel;

namespace Crossroads.Utils.Extensions;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        return value.GetType()
            .GetField(value.ToString())?
            .GetCustomAttributes(typeof (DescriptionAttribute), false)
            .SingleOrDefault() is not DescriptionAttribute attribute ? value.ToString() : attribute.Description;
    }
}