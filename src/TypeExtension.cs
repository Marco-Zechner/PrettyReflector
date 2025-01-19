using System.Reflection;
using MarcoZechner.ColorString;

namespace MarcoZechner.PrettyReflector;

public static class TypeExtension {
    private static readonly Dictionary<Type, string> primitiveTypes = new()
    {
        { typeof(bool), "bool" },
        { typeof(byte), "byte" },
        { typeof(char), "char" },
        { typeof(decimal), "decimal" },
        { typeof(double), "double" },
        { typeof(float), "float" },
        { typeof(int), "int" },
        { typeof(long), "long" },
        { typeof(sbyte), "sbyte" },
        { typeof(short), "short" },
        { typeof(string), "string" },
        { typeof(uint), "uint" },
        { typeof(ulong), "ulong" },
        { typeof(ushort), "ushort" },
    };

    public static string PrettyType(this Type type )
    {
        if (type.IsGenericType)
        {
            if (type.FullName?.StartsWith("System.ValueTuple") == true)
            {
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

                // Check for long tuple nesting
                var fieldTypes = fields.Select(f => f.FieldType.PrettyType()).ToList();
                if (fields.Length == 8 && fields[7].Name == "Rest" && fields[7].FieldType.FullName?.StartsWith("System.ValueTuple") == true)
                {
                    // Unnest the "Rest" field
                    fieldTypes = [.. fields.Take(7).Select(f => f.FieldType.PrettyType()), .. FlattenTupleTypes(fields[7].FieldType)];
                }

                return $"({string.Join(", ", fieldTypes)})";
            }

            var genArgs = type.GetGenericArguments();
            string typeName = type.Name.Split('`')[0]; // Remove generic arity suffix
            return $"{typeName}<{string.Join(", ", genArgs.Select(PrettyType))}>";
        }

        return primitiveTypes.TryGetValue(type, out string? primitiveName) ? primitiveName : type.Name;
    }

    public static ColoredString ColoredPrettyType(this Type type) {
        if (type.IsGenericType)
        {
            if (type.FullName?.StartsWith("System.ValueTuple") == true)
            {
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

                // Check for long tuple nesting
                var fieldTypes = fields.Select(f => f.FieldType.ColoredPrettyType()).ToList();
                if (fields.Length == 8 && fields[7].Name == "Rest" && fields[7].FieldType.FullName?.StartsWith("System.ValueTuple") == true)
                {
                    // Unnest the "Rest" field
                    fieldTypes = [.. fields.Take(7).Select(f => f.FieldType.ColoredPrettyType()), .. ColoredFlattenTupleTypes(fields[7].FieldType)];
                }

                return "(" + ColoredString.Join(", ", fieldTypes) + ")";
            }

            var genArgs = type.GetGenericArguments();
            string typeName = type.Name.Split('`')[0]; // Remove generic arity suffix
            return Color.Green.For(typeName) + "<" + ColoredString.Join(", ", genArgs.Select(ColoredPrettyType)) + ">";
        }

        return primitiveTypes.TryGetValue(type, out string? primitiveName) ? Color.Blue.For(primitiveName) : Color.Green.For(type.Name);
    }

    private static IEnumerable<string> FlattenTupleTypes(Type type)
    {
        if (!type.FullName?.StartsWith("System.ValueTuple") == true)
        {
            yield return type.PrettyType();
            yield break;
        }

        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        for (int i = 0; i < fields.Length; i++)
        {
            var fieldType = fields[i].FieldType;
            if (i == 7 && fields[i].Name == "Rest" && fieldType.FullName?.StartsWith("System.ValueTuple") == true)
            {
                foreach (var nestedType in FlattenTupleTypes(fieldType))
                {
                    yield return nestedType;
                }
            }
            else
            {
                yield return fieldType.PrettyType();
            }
        }
    }

    private static IEnumerable<ColoredString> ColoredFlattenTupleTypes(Type type)
    {
        if (!type.FullName?.StartsWith("System.ValueTuple") == true)
        {
            yield return type.ColoredPrettyType();
            yield break;
        }

        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        for (int i = 0; i < fields.Length; i++)
        {
            var fieldType = fields[i].FieldType;
            if (i == 7 && fields[i].Name == "Rest" && fieldType.FullName?.StartsWith("System.ValueTuple") == true)
            {
                foreach (var nestedType in ColoredFlattenTupleTypes(fieldType))
                {
                    yield return nestedType;
                }
            }
            else
            {
                yield return fieldType.ColoredPrettyType();
            }
        }
    }
}
