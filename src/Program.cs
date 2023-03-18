using Lox;

if (args.Length == 0)
{
    LoxInterpreter.RunPrompt();
}
else if (args.Length == 1)
{
    if (args[0] == "--help")
        LoxInterpreter.RunHelp();
    else if (args[0] == "--debug")
        LoxInterpreter.RunDebugPrompt();
    else
        LoxInterpreter.RunFile(args[0]);
}
else
{
    LoxInterpreter.RunHelp();
    Environment.Exit(64);
}
