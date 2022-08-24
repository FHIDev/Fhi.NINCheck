namespace Fhi.NINCheck;

public static class Extensions
{
    /// <summary>
    ///     Returns whether the provided string is comprised of numbers or not.
    /// </summary>
    /// <param name="input">The string to examine for numericity.</param>
    /// <returns>Whether the provided string is comprised of numbers.</returns>
    public static bool IsNumeric(this string input) => input.ToCharArray().All(char.IsNumber);


    /// <summary>
    ///     Converts a given string to an integer.
    /// </summary>
    /// <param name="input">The string to convert.</param>
    /// <returns>A valid integer, 0 if the input is not a usable number.</returns>
    public static int ToInt(this string input)
    {
        var ret = 0;

        if (input.Exists())
            int.TryParse(input, out ret);

        return ret;
    }

    /// <summary>
    ///     Returns whether the provided string contains useful content or not.
    /// </summary>
    /// <param name="input">The string to examine.</param>
    /// <returns>Whether the provided string 'Exists'.</returns>
    public static bool Exists(this string? input)
    {
        return !(input == null || string.IsNullOrWhiteSpace(input.Trim('\0')));
    }


    /// <summary>
    ///     Formats the provided string using the standard formatting syntax and the provided parameters.
    /// </summary>
    /// <param name="stringToFormat">The formatting string to use.</param>
    /// <param name="parameters">The paramaters to get values from.</param>
    /// <returns>A formatted copy of the string using the provided parameters and the standard formatting syntax.</returns>
    public static string FormatWith(this string stringToFormat, params object[] parameters) => string.Format(stringToFormat, parameters);
}