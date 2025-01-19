using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using MarcoZechner.ColorString;

namespace MarcoZechner.PrettyReflector; 

public static class ObjectExtension{
    public static string PrettyValue(this object? obj)
    {
        if (obj == null)
            return "null";
        var objType = obj.GetType();

        switch (obj)
        {
            case string str:
                return $"\"{str}\"";
            case char ch:
                return $"'{ch}'";
            case IDictionary dictionary:
                return PrettyDictionaryData(dictionary);
            case ITuple tuple:
                return PrettyTupleData(obj, objType);
            case IEnumerable enumerable:
                return $"[{string.Join(", ", enumerable.Cast<object>().Select(PrettyValue))}]";
        }

        if (objType.IsPrimitive)
            return PrettyNumbers(obj, objType);

        if (objType.IsEnum)
            return $"{objType.PrettyType()}.{obj}";

        return PrettyCustomTypes(obj, objType);
    }

    public static ColoredString ColoredPrettyValue(this object? obj)
    {
        if (obj == null)
            return Color.Blue.For("null");
        var objType = obj.GetType();

        switch (obj)
        {
            case string str:
                return Color.DarkYellow.For($"\"{str}\"");
            case char ch:
                return Color.DarkYellow.For($"'{ch}'");
            case IDictionary dictionary:
                return ColoredPrettyDictionaryData(dictionary);
            case ITuple tuple:
                return ColoredPrettyTupleData(obj, objType);
            case IEnumerable enumerable:
                return "[" + ColoredString.Join(", ", enumerable.Cast<object>().Select(ColoredPrettyValue)) + "]";
        }

        if (objType.IsPrimitive)
            return Color.Cyan.For(PrettyNumbers(obj, objType));

        if (objType.IsEnum)
            return objType.ColoredPrettyType() + "." + Color.Cyan.For(obj.ToString() ?? "null");

        return ColoredPrettyCustomTypes(obj, objType);
    }

    private static string PrettyCustomTypes(object obj, Type objType)
    {
        var properties = objType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead)
                    .Select(p =>
                        $"{p.PropertyType.PrettyType()} {p.Name} = {PrettyValue(p.GetValue(obj))}");

        var fields = objType.GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Select(f =>
                $"{f.FieldType.PrettyType()} {f.Name} = {PrettyValue(f.GetValue(obj))}");

        var members = string.Join(", ", properties.Concat(fields));
        return $"{objType.PrettyType()} {{ {members} }}";
    }

    private static ColoredString ColoredPrettyCustomTypes(object obj, Type objType)
    {
        IEnumerable<ColoredString> properties = objType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead)
                    .Select(p =>
                        p.PropertyType.ColoredPrettyType() + " " + Color.Magenta.For(p.Name) + " = " + ColoredPrettyValue(p.GetValue(obj)));

        IEnumerable<ColoredString> fields = objType.GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Select(f =>
                f.FieldType.ColoredPrettyType() + " " + f.Name + " = " + ColoredPrettyValue(f.GetValue(obj)));

        var members = ColoredString.Join(", ", properties.Concat(fields));
        return objType.ColoredPrettyType() + " { " + members + " }";
    }

    private static string PrettyNumbers(object obj, Type objType)
    {
        string formattedValue = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:G}", obj);

        // Append suffix based on the type
        return objType switch
        {
            _ when obj is float => $"{formattedValue}f",
            _ when obj is double => $"{formattedValue}d",
            _ when obj is decimal => $"{formattedValue}m",
            _ when obj is long => $"{formattedValue}L",
            _ when obj is uint => $"{formattedValue}u",
            _ when obj is ulong => $"{formattedValue}uL",
            _ => formattedValue // No suffix for int, short, byte, etc.
        };
    }

    private static string PrettyTupleData(object obj, Type objType)
    {
        var fields = objType.GetFields(BindingFlags.Public | BindingFlags.Instance);

        // Check for long tuple nesting
        var fieldValues = fields.Select(f => f.GetValue(obj)).ToList();
        if (fields.Length == 8 && fields[7].Name == "Rest" && fieldValues[7]?.GetType().FullName?.StartsWith("System.ValueTuple") == true)
        {
            // Unnest the "Rest" field
            fieldValues = [.. fields.Take(7).Select(f => f.GetValue(obj)), .. FlattenTuple(fieldValues[7])];
        }

        return $"({string.Join(", ", fieldValues.Select(PrettyValue))})";
    }
    
    private static ColoredString ColoredPrettyTupleData(object obj, Type objType)
    {
        var fields = objType.GetFields(BindingFlags.Public | BindingFlags.Instance);

        // Check for long tuple nesting
        var fieldValues = fields.Select(f => f.GetValue(obj)).ToList();
        if (fields.Length == 8 && fields[7].Name == "Rest" && fieldValues[7]?.GetType().FullName?.StartsWith("System.ValueTuple") == true)
        {
            // Unnest the "Rest" field
            fieldValues = [.. fields.Take(7).Select(f => f.GetValue(obj)), .. FlattenTuple(fieldValues[7])];
        }

        return "(" + ColoredString.Join(", ", fieldValues.Select(ColoredPrettyValue)) + ")";
    }

    private static string PrettyDictionaryData(IDictionary dictionary)
    {
        var entries = dictionary.Cast<object>()
                                .Select(entry =>
                                {
                                    var kvp = (dynamic)entry; // Dynamically access Key and Value
                                    return $"{PrettyValue(kvp.Key)}: {PrettyValue(kvp.Value)}";
                                });

        return $"[{string.Join(", ", entries)}]";
    }

    private static string ColoredPrettyDictionaryData(IDictionary dictionary)
    {
        var entries = (IEnumerable<ColoredString>) dictionary.Cast<object>()
                                .Select(entry =>
                                {
                                    var kvp = (dynamic)entry; // Dynamically access Key and Value
                                    return ColoredPrettyValue(kvp.Key) + ": " + ColoredPrettyValue(kvp.Value);
                                });

        return "[" + ColoredString.Join(", ", entries) + "]";
    }

    private static IEnumerable<object?> FlattenTuple(object? tuple)
    {
        if (tuple == null)
            yield break;

        var tupleType = tuple.GetType();
        if (!tupleType.FullName?.StartsWith("System.ValueTuple") == true)
        {
            yield return tuple;
            yield break;
        }

        var fields = tupleType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        for (int i = 0; i < fields.Length; i++)
        {
            var value = fields[i].GetValue(tuple);
            if (i == 7 && fields[i].Name == "Rest" && value?.GetType().FullName?.StartsWith("System.ValueTuple") == true)
            {
                foreach (var nestedValue in FlattenTuple(value))
                {
                    yield return nestedValue;
                }
            }
            else
            {
                yield return value;
            }
        }
    }
}
