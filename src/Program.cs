using Lox;
using Lox.Ast;

if (args.Length == 0)
{
    LoxEntryPoint.RunPrompt();
}
else if (args.Length == 1)
{
    if (args[0] == "--help")
        LoxEntryPoint.RunHelp();
    else if (args[0] == "--debug")
        LoxEntryPoint.RunDebugPrompt();
    else
        LoxEntryPoint.RunFile(args[0]);
}
else
{
    LoxEntryPoint.RunHelp();
    Environment.Exit(64);
}

