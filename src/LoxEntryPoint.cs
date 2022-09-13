using System.Text;

namespace Lox;

public static class LoxEntryPoint
{
    private static bool _hadError;

    public static void RunPrompt()
    {
        while (true)
        {
            Console.Write("> ");
            var line = Console.ReadLine();
            if (line == null)
                break;
            Run(line);
            _hadError = false;
        }
    }

    public static void RunFile(string filePath)
    {
        var content = System.IO.File.ReadAllText(filePath, Encoding.UTF8);
        Run(content);

        if (_hadError)
            Environment.Exit(65);
    }

    public static void RunHelp()
    {
        Console.WriteLine("Usage: cslox [script]");
    }

    public static void Run(string program)
    {
        Console.WriteLine("Running the program: " + program);
        var scanner = new Scanner(program);

        foreach (var token in scanner.ScanTokens())
        {
            Console.WriteLine(token);
        }
    }

    public static void RunDebugPrompt()
    {
        throw new System.NotImplementedException();
    }

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    public static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
        _hadError = true;
    }
}