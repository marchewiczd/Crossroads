using System.Text.RegularExpressions;

namespace Crossroads.Utils.Helpers;

public static partial class RegexHelper
{
    [GeneratedRegex("(\\d{4}|443|80)-")]
    public static partial Regex Port();
}