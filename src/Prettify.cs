using System.Linq.Expressions;

namespace MarcoZechner.PrettyReflector;

public abstract class Prettify{
    public static string Variable(Type type, string name, object? value)
    {
        return $"{type.PrettyType()} {name} = {value.PrettyValue()}";
    }

    public static string Variable<T>(Expression<Func<T>> variableExpression)
    {
        if (variableExpression.Body is MemberExpression member)
        {
            string memberPath = GetMemberPath(member);
            T value = variableExpression.Compile()();
            return $"{typeof(T).PrettyType()} {memberPath} = {value.PrettyValue()}";
        }

        if (variableExpression.Body is MethodCallExpression methodCall)
        {
            string variableName = GetMethodCallPath(methodCall);
            T value = variableExpression.Compile()();
            return $"{typeof(T).PrettyType()} {variableName} = {value.PrettyValue()}";
        }

        throw new InvalidOperationException("Invalid expression. Ensure you pass a variable reference.");
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

