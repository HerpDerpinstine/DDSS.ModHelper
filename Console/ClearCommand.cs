namespace DDSS_ModHelper.Console
{
    internal class ClearCommand : ConsoleCommand
    {
        public override string GetName()
            => "clear";
        public override string GetDescription() 
            => "Clears Console Log History";

        public override void Execute(string[] args)
        {
            ConsoleManager._consoleHistory = string.Empty;
            ConsoleManager.ApplyConsoleTextToObject();
        }
    }
}
