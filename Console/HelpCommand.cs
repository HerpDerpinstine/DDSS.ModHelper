namespace DDSS_ModHelper.Console
{
    public class HelpCommand : ConsoleCommand
    {
        public override string GetName()
            => "help";
        public override string GetDescription() 
            => "Shows All Commands";

        public override void Execute(string[] args)
        {
            foreach (ConsoleCommand command in ConsoleManager._cmds.Values)
                ConsoleManager.PrintMsg(command.GetName() + " - " + command.GetDescription());
        }
    }
}
