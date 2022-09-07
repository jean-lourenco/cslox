using Lox;

if (args.Length == 0)
{
    LoxEntryPoint.RunPrompt();
}
else if (args.Length == 1)
{
    if (args[0] == "--help")
        LoxEntryPoint.RunHelp();
    else
        LoxEntryPoint.RunFile(args[0]);
}
else
{
    LoxEntryPoint.RunHelp();
    Environment.Exit(64);
}

