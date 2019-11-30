namespace Cottle
{
    internal class Command
    {
        public static readonly Command NoOp = new Command { Type = CommandType.None };

        public string[] Arguments;

        public Command Body;

        public string Key;

        public StoreMode Mode;

        public string Name;

        public Command Next;

        public Expression Operand;

        public string Text;

        public CommandType Type;
    }
}