using System.Collections.Generic;

namespace Cottle
{
    internal class Statement
    {
        public static Statement CreateAssignFunction(string key, IReadOnlyList<string> arguments, StoreMode mode,
            Statement body)
        {
            return new Statement(StatementType.AssignFunction, arguments, body, key, mode, default, default, default);
        }

        public static Statement CreateAssignRender(string key, StoreMode mode, Statement body)
        {
            return new Statement(StatementType.AssignRender, default, body, key, mode, default, default, default);
        }

        public static Statement CreateAssignValue(string key, StoreMode mode, Expression operand)
        {
            return new Statement(StatementType.AssignValue, default, default, key, mode, default, operand, default);
        }

        public static Statement CreateComposite(Statement body, Statement next)
        {
            return new Statement(StatementType.Composite, default, body, default, default, next, default, default);
        }

        public static Statement CreateDump(Expression operand)
        {
            return new Statement(StatementType.Dump, default, default, default, default, default, operand, default);
        }

        public static Statement CreateEcho(Expression operand)
        {
            return new Statement(StatementType.Echo, default, default, default, default, default, operand, default);
        }

        public static Statement CreateFor(string key, string value, Expression operand, Statement body, Statement next)
        {
            return new Statement(StatementType.For, default, body, key, default, next, operand, value);
        }

        public static Statement CreateIf(Expression operand, Statement body, Statement next)
        {
            return new Statement(StatementType.If, default, body, default, default, next, operand, default);
        }

        public static Statement CreateLiteral(string value)
        {
            return new Statement(StatementType.Literal, default, default, default, default, default, default, value);
        }

        public static Statement CreateReturn(Expression operand)
        {
            return new Statement(StatementType.Return, default, default, default, default, default, operand, default);
        }

        public static Statement CreateUnwrap(Statement body)
        {
            return new Statement(StatementType.Unwrap, default, body, default, default, default, default, default);
        }

        public static Statement CreateWhile(Expression operand, Statement body)
        {
            return new Statement(StatementType.While, default, body, default, default, default, operand, default);
        }

        public static Statement CreateWrap(Expression operand, Statement body)
        {
            return new Statement(StatementType.Wrap, default, body, default, default, default, operand, default);
        }

        public static readonly Statement NoOp = new Statement(StatementType.None, default, default, default, default,
            default, default, default);

        public readonly IReadOnlyList<string> Arguments;

        public readonly Statement Body;

        public readonly string Key;

        public readonly StoreMode Mode;

        public readonly Statement Next;

        public readonly Expression Operand;

        public readonly StatementType Type;

        public readonly string Value;

        private Statement(StatementType type, IReadOnlyList<string> arguments, Statement body, string key,
            StoreMode mode, Statement next, Expression operand, string value)
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