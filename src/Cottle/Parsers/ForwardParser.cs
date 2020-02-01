using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Cottle.Builtins;
using Cottle.Parsers.Forward;
using Cottle.Values;

namespace Cottle.Parsers
{
    internal class ForwardParser : IParser
    {
        private delegate bool ParseKeyword(ForwardParser parser, out Command command,
            out IEnumerable<DocumentReport> reports);

        private static readonly Dictionary<string, ParseKeyword> Keywords = new Dictionary<string, ParseKeyword>
        {
            ["_"] = (ForwardParser p, out Command c, out IEnumerable<DocumentReport> f) =>
                p.TryCreateComment(out c, out f),
            ["declare"] = (ForwardParser p, out Command c, out IEnumerable<DocumentReport> f) =>
                p.TryCreateDeclare(out c, out f),
            ["define"] = (ForwardParser p, out Command c, out IEnumerable<DocumentReport> f) =>
                p.TryCreateSet(out c, out f),
            ["dump"] = (ForwardParser p, out Command c, out IEnumerable<DocumentReport> f) =>
                p.TryCreateDump(out c, out f),
            ["echo"] = (ForwardParser p, out Command c, out IEnumerable<DocumentReport> f) =>
                p.TryCreateEcho(out c, out f),
            ["for"] = (ForwardParser p, out Command c, out IEnumerable<DocumentReport> f) =>
                p.TryCreateFor(out c, out f),
            ["if"] = (ForwardParser p, out Command c, out IEnumerable<DocumentReport> f) =>
                p.TryCreateIfThen(out c, out f),
            ["return"] = (ForwardParser p, out Command c, out IEnumerable<DocumentReport> f) =>
                p.TryCreateReturn(out c, out f),
            ["set"] = (ForwardParser p, out Command c, out IEnumerable<DocumentReport> f) =>
                p.TryCreateSet(out c, out f),
            ["while"] = (ForwardParser p, out Command c, out IEnumerable<DocumentReport> f) =>
                p.TryCreateWhile(out c, out f)
        };

        private readonly Lexer _lexer;
        private readonly Func<string, string> _trimmer;

        public ForwardParser(string blockBegin, string blockContinue, string blockEnd, char escape,
            Func<string, string> trimmer)
        {
            _lexer = new Lexer(blockBegin, blockContinue, blockEnd, escape);
            _trimmer = trimmer;
        }

        public bool Parse(TextReader reader, out Command command, out IEnumerable<DocumentReport> reports)
        {
            _lexer.Reset(reader);
            _lexer.NextRaw();

            if (!TryParseCommand(out command, out reports))
                return false;

            if (_lexer.Current.Type != LexemType.EndOfFile)
            {
                reports = CreateReports("end of file");

                return false;
            }

            reports = default;

            return true;
        }

        private static Expression BuildInvoke(IFunction function, params Expression[] arguments)
        {
            var source = Expression.CreateConstant(new FunctionValue(function));

            return Expression.CreateInvoke(source, arguments);
        }

        private bool TryCreateComment(out Command command, out IEnumerable<DocumentReport> reports)
        {
            do
            {
                _lexer.NextRaw();
            } while (_lexer.Current.Type == LexemType.Text);

            command = null;
            reports = default;

            return true;
        }

        private bool TryCreateDeclare(out Command command, out IEnumerable<DocumentReport> reports)
        {
            return TryParseAssignment(StoreMode.Local, out command, out reports);
        }

        private bool TryCreateDump(out Command command, out IEnumerable<DocumentReport> reports)
        {
            return TryParseCommandOperand(Command.CreateDump, out command, out reports);
        }

        private bool TryCreateEcho(out Command command, out IEnumerable<DocumentReport> reports)
        {
            return TryParseCommandOperand(Command.CreateEcho, out command, out reports);
        }

        private bool TryCreateFor(out Command command, out IEnumerable<DocumentReport> reports)
        {
            if (!TryParseSymbol(out var element, out reports))
            {
                command = default;

                return false;
            }

            string key;
            string value;

            if (_lexer.Current.Type == LexemType.Comma)
            {
                _lexer.NextBlock();

                if (!TryParseSymbol(out value, out reports))
                {
                    command = default;

                    return false;
                }

                key = element;
            }
            else
            {
                key = string.Empty;
                value = element;
            }

            if (!TryParseExpected(LexemType.Symbol, "in", "'in' keyword", out reports) ||
                !TryParseExpression(out var source, out reports) ||
                !TryParseCommandBody(out var body, out reports))
            {
                command = default;

                return false;
            }

            Command empty;

            if (_lexer.Current.Type == LexemType.BlockContinue)
            {
                _lexer.NextBlock();

                if (!TryParseExpected(LexemType.Symbol, "empty", "'empty' keyword", out reports) ||
                    !TryParseCommandBody(out empty, out reports))
                {
                    command = default;

                    return false;
                }
            }
            else
                empty = Command.NoOp;

            command = Command.CreateFor(key, value, source, body, empty);
            reports = default;

            return true;
        }

        private bool TryCreateIfThen(out Command command, out IEnumerable<DocumentReport> reports)
        {
            if (!TryParseExpression(out var ifCondition, out reports) ||
                !TryParseCommandBody(out var ifBody, out reports))
            {
                command = null;

                return false;
            }

            var branches = new List<(Expression, Command)>();
            var final = Command.NoOp;
            var next = true;

            branches.Add((ifCondition, ifBody));

            while (next && _lexer.Current.Type == LexemType.BlockContinue)
            {
                _lexer.NextBlock();

                switch (_lexer.Current.Type == LexemType.Symbol ? _lexer.Current.Value : string.Empty)
                {
                    case "elif":
                        _lexer.NextBlock();

                        if (!TryParseExpression(out var elifCondition, out reports) ||
                            !TryParseCommandBody(out var elifBody, out reports))
                        {
                            command = default;

                            return false;
                        }

                        branches.Add((elifCondition, elifBody));

                        break;

                    case "else":
                        _lexer.NextBlock();

                        if (!TryParseCommandBody(out var elseBody, out reports))
                        {
                            command = default;

                            return false;
                        }

                        final = elseBody;
                        next = false;

                        break;

                    default:
                        command = default;
                        reports = CreateReports("'elif' or 'else' keyword");

                        return false;
                }
            }

            for (var i = branches.Count - 1; i >= 0; --i)
                final = Command.CreateIf(branches[i].Item1, branches[i].Item2, final);

            command = final;
            reports = default;

            return true;
        }

        private bool TryCreateReturn(out Command command, out IEnumerable<DocumentReport> reports)
        {
            return TryParseCommandOperand(Command.CreateReturn, out command, out reports);
        }

        private bool TryCreateSet(out Command command, out IEnumerable<DocumentReport> reports)
        {
            return TryParseAssignment(StoreMode.Global, out command, out reports);
        }

        private bool TryCreateWhile(out Command command, out IEnumerable<DocumentReport> reports)
        {
            if (!TryParseExpression(out var condition, out reports) ||
                !TryParseCommandBody(out var body, out reports))
            {
                command = default;

                return false;
            }

            command = Command.CreateWhile(condition, body);

            return true;
        }


        private bool TryParseAssignment(StoreMode mode, out Command command, out IEnumerable<DocumentReport> reports)
        {
            List<string> arguments;

            if (!TryParseSymbol(out var name, out reports))
            {
                command = default;

                return false;
            }

            // Parse function arguments if provided
            if (_lexer.Current.Type == LexemType.ParenthesisBegin)
            {
                arguments = new List<string>();

                for (_lexer.NextBlock(); _lexer.Current.Type != LexemType.ParenthesisEnd;)
                {
                    if (!TryParseSymbol(out var symbol, out reports))
                    {
                        command = default;

                        return false;
                    }

                    arguments.Add(symbol);

                    if (_lexer.Current.Type == LexemType.Comma)
                        _lexer.NextBlock();
                }

                _lexer.NextBlock();
            }
            else
                arguments = null;

            // Early exit if no body, render nor value is defined
            if (_lexer.Current.Type != LexemType.Symbol)
            {
                _lexer.NextRaw();

                // Arguments were defined, build function assignment
                if (arguments != null)
                {
                    command = Command.CreateAssignFunction(name, arguments, mode, Command.NoOp);
                    reports = default;

                    return true;
                }

                // Arguments where not defined, build value assignment
                command = Command.CreateAssignValue(name, mode, Expression.Void);
                reports = default;

                return true;
            }

            // Parse 'as' or 'to' keyword
            if (mode == StoreMode.Global)
            {
                // FIXME: raise "notice" event, then remove legacy keyword handling
                if (_lexer.Current.Value == "as")
                {
                    _lexer.NextBlock();

                    mode = StoreMode.Local;
                }
                else
                {
                    if (!TryParseExpected(LexemType.Symbol, "to", "'to' keyword", out reports))
                    {
                        command = default;

                        return false;
                    }
                }
            }
            else if (!TryParseExpected(LexemType.Symbol, "as", "'as' keyword", out reports))
            {
                command = default;

                return false;
            }

            // Arguments were defined, build function assignment
            if (arguments != null)
            {
                if (!TryParseCommandBody(out var body, out reports))
                {
                    command = default;

                    return false;
                }

                command = Command.CreateAssignFunction(name, arguments, mode, body);

                return true;
            }

            // No arguments provided and literal body follows, build render assignment
            if (_lexer.Current.Type == LexemType.Colon)
            {
                if (!TryParseCommandBody(out var body, out reports))
                {
                    command = default;

                    return false;
                }

                command = Command.CreateAssignRender(name, mode, body);

                return true;
            }

            // No arguments and no literal body, build value assignment
            if (!TryParseExpression(out var operand, out reports))
            {
                command = default;

                return false;
            }

            _lexer.NextRaw();

            command = Command.CreateAssignValue(name, mode, operand);

            return true;
        }

        private bool TryParseCommand(out Command command, out IEnumerable<DocumentReport> reports)
        {
            var commands = new List<Command>();

            while
            (
                _lexer.Current.Type != LexemType.BlockContinue &&
                _lexer.Current.Type != LexemType.BlockEnd &&
                _lexer.Current.Type != LexemType.EndOfFile
            )
            {
                // Parse next command or exit loop
                switch (_lexer.Current.Type)
                {
                    case LexemType.BlockBegin:
                        _lexer.NextBlock();

                        if (_lexer.Current.Type == LexemType.Symbol &&
                            ForwardParser.Keywords.TryGetValue(_lexer.Current.Value, out var parse))
                        {
                            _lexer.NextBlock();
                        }
                        else
                        {
                            parse = (ForwardParser p, out Command c, out IEnumerable<DocumentReport> f) =>
                                p.TryCreateEcho(out c, out f);
                        }

                        if (!parse(this, out var blockCommand, out reports))
                        {
                            command = default;

                            return false;
                        }

                        if (_lexer.Current.Type != LexemType.BlockEnd)
                        {
                            command = default;
                            reports = CreateReports("end of block");

                            return false;
                        }

                        // Ignore empty commands
                        if (blockCommand != null)
                            commands.Add(blockCommand);

                        _lexer.NextRaw();

                        break;

                    case LexemType.Text:
                        commands.Add(Command.CreateLiteral(_trimmer(_lexer.Current.Value)));

                        _lexer.NextRaw();

                        break;

                    default:
                        command = default;
                        reports = CreateReports("text or block begin ('{')");

                        return false;
                }
            }

            if (commands.Count < 1)
                command = Command.NoOp;
            else if (commands.Count == 1)
                command = commands[0];
            else
            {
                var composite = commands[commands.Count - 1];

                for (var i = commands.Count - 2; i >= 0; --i)
                    composite = Command.CreateComposite(commands[i], composite);

                command = composite;
            }

            reports = default;

            return true;
        }

        private bool TryParseCommandBody(out Command command, out IEnumerable<DocumentReport> reports)
        {
            if (_lexer.Current.Type != LexemType.Colon)
            {
                command = default;
                reports = CreateReports("body separator (':')");

                return false;
            }

            _lexer.NextRaw();

            return TryParseCommand(out command, out reports);
        }

        private bool TryParseCommandOperand(Func<Expression, Command> constructor, out Command command,
            out IEnumerable<DocumentReport> reports)
        {
            if (!TryParseExpression(out var operand, out reports))
            {
                command = default;

                return false;
            }

            _lexer.NextRaw();

            command = constructor(operand);

            return true;
        }

        private bool TryParseExpected(LexemType type, string value, string message,
            out IEnumerable<DocumentReport> reports)
        {
            if (_lexer.Current.Type != type || _lexer.Current.Value != value)
            {
                reports = CreateReports(message);

                return false;
            }

            _lexer.NextBlock();

            reports = default;

            return true;
        }

        private bool TryParseExpression(out Expression expression, out IEnumerable<DocumentReport> reports)
        {
            var operands = new Stack<Expression>();
            var operators = new Stack<Operator>();

            while (true)
            {
                if (!TryParseValue(out var operand, out reports))
                {
                    expression = default;

                    return false;
                }

                operands.Push(operand);

                Operator current;

                switch (_lexer.Current.Type)
                {
                    case LexemType.DoubleAmpersand:
                        current = new Operator(BuiltinOperators.OperatorAnd, 0);

                        break;

                    case LexemType.DoublePipe:
                        current = new Operator(BuiltinOperators.OperatorOr, 0);

                        break;

                    case LexemType.Equal:
                        current = new Operator(BuiltinOperators.OperatorEqual, 1);

                        break;

                    case LexemType.GreaterEqual:
                        current = new Operator(BuiltinOperators.OperatorGreaterEqual, 1);

                        break;

                    case LexemType.GreaterThan:
                        current = new Operator(BuiltinOperators.OperatorGreaterThan, 1);

                        break;

                    case LexemType.LowerEqual:
                        current = new Operator(BuiltinOperators.OperatorLowerEqual, 1);

                        break;

                    case LexemType.LowerThan:
                        current = new Operator(BuiltinOperators.OperatorLowerThan, 1);

                        break;

                    case LexemType.Minus:
                        current = new Operator(BuiltinOperators.OperatorSub, 2);

                        break;

                    case LexemType.NotEqual:
                        current = new Operator(BuiltinOperators.OperatorNotEqual, 1);

                        break;

                    case LexemType.Percent:
                        current = new Operator(BuiltinOperators.OperatorMod, 3);

                        break;

                    case LexemType.Plus:
                        current = new Operator(BuiltinOperators.OperatorAdd, 2);

                        break;

                    case LexemType.Slash:
                        current = new Operator(BuiltinOperators.OperatorDiv, 3);

                        break;

                    case LexemType.Star:
                        current = new Operator(BuiltinOperators.OperatorMul, 3);

                        break;

                    default:
                        while (operators.Count > 0)
                        {
                            var remaining = operators.Pop();
                            var value = operands.Pop();

                            operands.Push(ForwardParser.BuildInvoke(remaining.Function, operands.Pop(), value));
                        }

                        expression = operands.Pop();
                        reports = default;

                        return true;
                }

                _lexer.NextBlock();

                while (operators.Count > 0 && operators.Peek().Precedence >= current.Precedence)
                {
                    var other = operators.Pop();
                    var value = operands.Pop();

                    operands.Push(ForwardParser.BuildInvoke(other.Function, operands.Pop(), value));
                }

                operators.Push(current);
            }
        }

        private bool TryParseSymbol(out string name, out IEnumerable<DocumentReport> reports)
        {
            if (_lexer.Current.Type != LexemType.Symbol)
            {
                reports = CreateReports("symbol (variable name)");
                name = default;

                return false;
            }

            reports = default;
            name = _lexer.Current.Value;

            _lexer.NextBlock();

            return true;
        }

        private bool TryParseValue(out Expression expression, out IEnumerable<DocumentReport> reports)
        {
            switch (_lexer.Current.Type)
            {
                case LexemType.Bang:
                    _lexer.NextBlock();

                    if (!TryParseValue(out var notExpression, out reports))
                    {
                        expression = default;

                        return false;
                    }

                    expression = ForwardParser.BuildInvoke(BuiltinOperators.OperatorNot, notExpression);
                    reports = default;

                    return true;

                case LexemType.BracketBegin:
                    var elements = new List<ExpressionElement>();
                    var index = 0;

                    for (_lexer.NextBlock(); _lexer.Current.Type != LexemType.BracketEnd;)
                    {
                        if (!TryParseExpression(out var element, out reports))
                        {
                            expression = default;

                            return false;
                        }

                        Expression key;
                        Expression value;

                        if (_lexer.Current.Type == LexemType.Colon)
                        {
                            _lexer.NextBlock();

                            if (!TryParseExpression(out value, out reports))
                            {
                                expression = default;

                                return false;
                            }

                            key = element;
                        }
                        else
                        {
                            key = Expression.CreateConstant(index++);
                            value = element;
                        }

                        elements.Add(new ExpressionElement(key, value));

                        if (_lexer.Current.Type == LexemType.Comma)
                            _lexer.NextBlock();
                    }

                    expression = Expression.CreateMap(elements);

                    _lexer.NextBlock();

                    break;

                case LexemType.Minus:
                    _lexer.NextBlock();

                    if (!TryParseValue(out var minusRhs, out reports))
                    {
                        expression = default;

                        return false;
                    }

                    var minusLhs = Expression.CreateConstant(0);

                    expression = ForwardParser.BuildInvoke(BuiltinOperators.OperatorSub, minusLhs, minusRhs);
                    reports = default;

                    return true;

                case LexemType.Number:
                    if (!decimal.TryParse(_lexer.Current.Value, NumberStyles.Number, CultureInfo.InvariantCulture,
                        out var number))
                        number = 0;

                    expression = Expression.CreateConstant(number);

                    _lexer.NextBlock();

                    break;

                case LexemType.ParenthesisBegin:
                    _lexer.NextBlock();

                    if (!TryParseExpression(out expression, out reports))
                        return false;

                    if (_lexer.Current.Type != LexemType.ParenthesisEnd)
                    {
                        reports = CreateReports("parenthesis end (')')");

                        return false;
                    }

                    _lexer.NextBlock();

                    reports = default;

                    return true;

                case LexemType.Plus:
                    _lexer.NextBlock();

                    return TryParseValue(out expression, out reports);

                case LexemType.String:
                    expression = Expression.CreateConstant(_lexer.Current.Value);

                    _lexer.NextBlock();

                    break;

                case LexemType.Symbol:
                    expression = Expression.CreateSymbol(_lexer.Current.Value);

                    _lexer.NextBlock();

                    break;

                default:
                    expression = default;
                    reports = CreateReports("expression");

                    return false;
            }

            while (true)
            {
                switch (_lexer.Current.Type)
                {
                    case LexemType.BracketBegin:
                        _lexer.NextBlock();

                        if (!TryParseExpression(out var subscript, out reports))
                            return false;

                        if (_lexer.Current.Type != LexemType.BracketEnd)
                        {
                            reports = CreateReports("array index end (']')");

                            return false;
                        }

                        _lexer.NextBlock();

                        expression = Expression.CreateAccess(expression, subscript);

                        break;

                    case LexemType.Dot:
                        _lexer.NextBlock();

                        if (_lexer.Current.Type != LexemType.Symbol)
                        {
                            reports = CreateReports("field name");

                            return false;
                        }

                        expression =
                            Expression.CreateAccess(expression, Expression.CreateConstant(_lexer.Current.Value));

                        _lexer.NextBlock();

                        break;

                    case LexemType.ParenthesisBegin:
                        var arguments = new List<Expression>();

                        for (_lexer.NextBlock(); _lexer.Current.Type != LexemType.ParenthesisEnd;)
                        {
                            if (!TryParseExpression(out var argument, out reports))
                                return false;

                            arguments.Add(argument);

                            if (_lexer.Current.Type == LexemType.Comma)
                                _lexer.NextBlock();
                        }

                        _lexer.NextBlock();

                        expression = Expression.CreateInvoke(expression, arguments);

                        break;

                    default:
                        reports = default;

                        return true;
                }
            }
        }

        private IReadOnlyList<DocumentReport> CreateReports(string expected)
        {
            var current = _lexer.Current;
            var message = $"expected {expected}, found {current.Value}";

            return new[] { new DocumentReport(DocumentSeverity.Error, current.Offset, current.Length, message) };
        }
    }
}