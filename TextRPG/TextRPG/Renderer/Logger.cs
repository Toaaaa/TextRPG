public class Logger
{
    public static void Write(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(message);
        Console.ResetColor();
    }
    public static void WriteLine(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void Debug(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        // Thread.Sleep(duration);
        Console.ResetColor();
        string input = Console.ReadLine();
    }
}