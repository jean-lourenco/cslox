using System.Globalization;

namespace Lox;

public static class GlobalConfig
{
    public static IFormatProvider DefaultNumberFormat = CultureInfo.GetCultureInfo("en-us");
}