using MarcoZechner.ColorString;
using MarcoZechner.PrettyReflector;

namespace MarcoZechner.Test;

public class Program {
    class TestClass {
        public int Number { get; set; }
        public required string Text { get; set; }
    }

    enum TestEnum {
        Value1 = 1,
        Value2 = 2,
        Value3 = 4
    }

    public static void Main()
    {
        var colorPalette = Color.Black.For("Black\n") 
            + Color.DarkBlue.For("DarkBlue\n") 
            + Color.DarkGreen.For("DarkGreen\n") 
            + Color.DarkCyan.For("DarkCyan\n") 
            + Color.DarkRed.For("DarkRed\n") 
            + Color.DarkMagenta.For("DarkMagenta\n") 
            + Color.DarkYellow.For("DarkYellow\n") 
            + Color.Gray.For("Gray\n") 
            + Color.DarkGray.For("DarkGray\n") 
            + Color.Blue.For("Blue\n") 
            + Color.Green.For("Green\n") 
            + Color.Cyan.For("Cyan\n") 
            + Color.Red.For("Red\n") 
            + Color.Magenta.For("Magenta\n") 
            + Color.Yellow.For("Yellow\n") 
            + Color.White.For("White\n");

        ColoredConsole.WriteLine(colorPalette);

        (int a, int b, int c, int d, int, int, int, int, int, int) tuple = (1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
        var prettyTuple = Prettify.ColoredVariable(() => tuple);
        ColoredConsole.WriteLine(prettyTuple);

        TestClass testObject = new() { Number = 42, Text = "Hello World!" };
        
        var prettyObject = Prettify.ColoredVariable(() => testObject);
        ColoredConsole.WriteLine(prettyObject);

        TestEnum testEnum = TestEnum.Value2;
        var prettyEnum = Prettify.ColoredVariable(() => testEnum);
        ColoredConsole.WriteLine(prettyEnum);

        TestEnum flagEnum = TestEnum.Value1 | TestEnum.Value3;
        var prettyFlagEnum = Prettify.ColoredVariable(() => flagEnum);
        ColoredConsole.WriteLine(prettyFlagEnum);

        Console.WriteLine("====== StringExtension Tests ======");
        Console.WriteLine(">>> ReplaceAt:");
        string testString = "Hello World!";
        Console.WriteLine(testString.ReplaceAt(6, "Universe"));

        Console.WriteLine(">>> ShortenLeft:");
        Console.WriteLine(testString.ShortenLeft(6));

        Console.WriteLine(">>> ShortenRight:");
        Console.WriteLine(testString.ShortenRight(5));

        Console.WriteLine(">>> SetLength:");
        Console.WriteLine(testString.SetLength(15, padLeft: true, paddingChar: '_'));
        Console.WriteLine(testString.SetLength(15, padLeft: false, paddingChar: '_'));
        Console.WriteLine(testString.SetLength(6, truncateLeft: true, paddingChar: '_'));
        Console.WriteLine(testString.SetLength(5, truncateLeft: false, paddingChar: '_'));

        Console.WriteLine(">>> Indent:");
        string indentedString = "Hello World!\nThis is my story.\nI have much to tell.";
        Console.WriteLine(indentedString.Indent(4, ' '));
        string arrowedString = indentedString.Indent("--> ");
        Console.WriteLine(arrowedString);

        Console.WriteLine(">>> CombineLines:");
        string leftSideString = "LeftTitle:";

        Console.WriteLine(leftSideString.CombineLines(arrowedString, "|"));
    }
}