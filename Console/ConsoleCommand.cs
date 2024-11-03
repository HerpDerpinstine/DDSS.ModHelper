namespace DDSS_ModHelper.Console
{
    public abstract class ConsoleCommand
    {
        public ConsoleCommand() { }
        public abstract string GetName();
        public abstract string GetDescription();
        public abstract void Execute(string[] args);
    }
}