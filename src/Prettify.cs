using System.Linq.Expressions;
using MarcoZechner.ColorString;

namespace MarcoZechner.PrettyReflector;

public abstract class Prettify{
    public static string Variable(Type type, string name, object? value)
    {
        return $"{type.PrettyType()} {name} = {value.PrettyValue()}";
    }

    public static ColoredString ColoredVariable(Type type, string name, object? value) {
        return type.ColoredPrettyType() + " " + Color.Magenta.For(name) + " = " + value.ColoredPrettyValue();
    }

    private static string GetVariableName<T>(Expression<Func<T>> variableExpression)
    {
        return variableExpression.Body switch
        {
            MemberExpression member => GetMemberPath(member),
            MethodCallExpression methodCall => GetMethodCallPath(methodCall),
            _ => throw new InvalidOperationException("Invalid expression. Ensure you pass a variable reference."),
        };
    }

    public static string Variable<T>(Expression<Func<T>> variableExpression)
    {
        string variableName = GetVariableName(variableExpression);
        T value = variableExpression.Compile().Invoke();
        return Variable(typeof(T), variableName, value);
    }

    public static ColoredString ColoredVariable<T>(Expression<Func<T>> variableExpression)
    {
        string variableName = GetVariableName(variableExpression);
        T value = variableExpression.Compile().Invoke();
        return ColoredVariable(typeof(T), variableName, value);
    }

    private static string GetMemberPath(MemberExpression? member)
    {
        var parts = new Stack<string>();

        // Traverse the member chain
        while (member != null)
        {
            parts.Push(member.Member.Name);
            member = member.Expression as MemberExpression;
        }

        return string.Join(".", parts); // Build the full path
    }

    private static string GetMethodCallPath(MethodCallExpression methodCall)
    {
        if (methodCall.Object is MemberExpression member)
        {
            string basePath = GetMemberPath(member);
            string arguments = string.Join(", ", methodCall.Arguments.Select(arg => Expression.Lambda(arg).Compile().DynamicInvoke()?.PrettyValue()));
            return $"{basePath}[{arguments}]";
        }
        throw new InvalidOperationException("Unable to process method call path.");
    }
}

