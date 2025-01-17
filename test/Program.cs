using MarcoZechner.PrettyReflector;

namespace MarcoZechner.Test;

public class Program {
    public static void Main()
    {
        (int a, int b, int c, int d, int, int, int, int, int, int) tuple = (1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
        var prettyTuple = Prettify.Variable(() => tuple);
        Console.WriteLine(prettyTuple);
    }
}