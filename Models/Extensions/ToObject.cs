using System.Reflection;

namespace Models;

public static class ToObjectExtensions
{
    public static T ToObject<T>(this IDictionary<string, object> dict)
    {
        if (dict == null) return Activator.CreateInstance<T>();
        if (typeof(T) is IDictionary<string, object>) return (T)dict;
        var obj = Activator.CreateInstance<T>();
        foreach (var kv in dict)
        {
            var prop = typeof(T).GetProperty(kv.Key);
            if (prop == null) continue;
            if (prop.PropertyType.IsEnum)
                prop.SetValue(obj, Enum.Parse(prop.PropertyType, kv.Value.ToString(), true));
            else
                SetValue(obj, prop, kv.Value); //    prop.SetValue(obj, kv.Value);
        }

        return obj;
    }

    public static IEnumerable<T> ToObject<T>(this IEnumerable<IDictionary<string, object>> list)
    {
        return typeof(T) == list.GetType() ? (IEnumerable<T>)list : list.Select(l => l.ToObject<T>());
    }

    public static void SetValue(this object inputObject, PropertyInfo propertyInfo, object propertyVal)
    {
        //Convert.ChangeType does not handle conversion to nullable types
        //if the property type is nullable, we need to get the underlying type of the property
        var targetType = IsNullableType(propertyInfo.PropertyType)
            ? Nullable.GetUnderlyingType(propertyInfo.PropertyType)
            : propertyInfo.PropertyType;

        if (propertyVal != null)
            propertyVal =
                Convert.ChangeType(propertyVal,
                    targetType); //Returns an System.Object with the specified System.Type and whose value is equivalent to the specified object.
        //Set the value of the property
        propertyInfo.SetValue(inputObject, propertyVal, null);
    }

    private static bool IsNullableType(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
    }
}