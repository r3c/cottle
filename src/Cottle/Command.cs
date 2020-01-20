using System.Collections.Generic;

namespace Cottle
{
    internal class Command
    {
        public static Command CreateAssignFunction(string key, IReadOnlyList<string> arguments, StoreMode mode,
            Command body)
        {
            return new Command(CommandType.AssignFunction, arguments, body, key, mode, default, default, default);
        }

        public static Command CreateAssignRender(string key, StoreMode mode, Command body)
        {
            return new Command(CommandType.AssignRender, default, body, key, mode, default, default, default);
        }

        public static Command CreateAssignValue(string key, StoreMode mode, Expression operand)
        {
            return new Command(CommandType.AssignValue, default, default, key, mode, default, operand, default);
        }

        public static Command CreateComposite(Command body, Command next)
        {
            return new Command(CommandType.Composite, default, body, default, default, next, default, default);
        }

        public static Command CreateDump(Expression operand)
        {
            return new Command(CommandType.Dump, default, default, default, default, default, operand, default);
        }

        public static Command CreateEcho(Expression operand)
        {
            return new Command(CommandType.Echo, default, default, default, default, default, operand, default);
        }

        public static Command CreateFor(string key, string value, Expression operand, Command body, Command next)
        {
            return new Command(CommandType.For, default, body, key, default, next, operand, value);
        }

        public static Command CreateIf(Expression operand, Command body, Command next)
        {
            return new Command(CommandType.If, default, body, default, default, next, operand, default);
        }

        public static Command CreateLiteral(string value)
        {
            return new Command(CommandType.Literal, default, default, default, default, default, default, value);
        }

        public static Command CreateReturn(Expression operand)
        {
            return new Command(CommandType.Return, default, default, default, default, default, operand, default);
        }

        public static Command CreateWhile(Expression operand, Command body)
        {
            return new Command(CommandType.While, default, body, default, default, default, operand, default);
        }

        public static readonly Command NoOp = new Command(CommandType.None, default, default, default, default,
            default, default, default);

        public readonly IReadOnlyList<string> Arguments;

        public readonly Command Body;

        public readonly string Key;

        public readonly StoreMode Mode;

        public readonly Command Next;

        public readonly Expression Operand;

        public readonly CommandType Type;

        public readonly string Value;

        private Command(CommandType type, IReadOnlyList<string> arguments, Command body, string key, StoreMode mode,
            Command next, Expression operand, string value)
        {
            Arguments = arguments;
            Body = body;
            Key = key;
            Mode = mode;
            Value = value;
            Next = next;
            Operand = operand;
            Type = type;
        }
    }
}