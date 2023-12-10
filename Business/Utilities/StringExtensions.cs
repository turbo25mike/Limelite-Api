using System;

namespace Business.Utilities;

public static class StringExtensions
{
    public static T TryParse<T>(this object input)
    {
        return input.TryParse(default(T));
    }

    public static T TryParse<T>(this object input, T defaultValue)
    {
        if (input == null) return defaultValue;
        if (input.GetType() == typeof(T)) return (T)input;
        var type = typeof(T);
        var parseMethod = type.GetMethod("Parse", new[] { typeof(string) });

        if (parseMethod != null)
            try
            {
                var value = parseMethod.Invoke(null, new[] { input.ToString() });
                return value is T ? (T)value : defaultValue;
            }
            catch (Exception)
            {
                return defaultValue;
            }

        return defaultValue;
    }

    public static bool TryParse<T>(this object input, out T output)
    {
        return input.TryParse(out output, default);
    }

    public static bool TryParse<T>(this object input, out T output, T defaultValue)
    {
        output = defaultValue;
        if (input == null) return false;

        if (input.GetType() == typeof(T))
        {
            output = (T)input;
            return true;
        }

        var type = typeof(T);
        var parseMethod = type.GetMethod("TryParse", new[] { typeof(string), typeof(T).MakeByRefType() });

        if (parseMethod != null)
        {
            object[] parameters = { input.ToString(), output };
            try
            {
                var value = parseMethod.Invoke(null, parameters);
                if (value is bool)
                {
                    var successful = (bool)value;
                    if (successful)
                    {
                        output = (T)parameters[1];
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        return false;
    }
}