using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Cottle.Builtins;
using Cottle.Parsers.Forward;

namespace Cottle.Parsers
{
    internal class ForwardParser : IParser
    {
        private static readonly Dictionary<string, Keyword> Keywords = new Dictionary<string, Keyword>
        {
            ["_"] = new Keyword((ForwardParser p, ParserState s, out Statement c) =>
                p.TryCreateComment(out c), false),
            ["declare"] = new Keyword((ForwardParser p, ParserState s, out Statement c) =>
                p.TryCreateDeclare(s, out c), true),
            ["define"] = new Keyword((ForwardParser p, ParserState s, out Statement c) =>
                p.TryCreateDefine(s, out c), true),
            ["dump"] = new Keyword((ForwardParser p, ParserState s, out Statement c) =>
                p.TryCreateDump(s, out c), true),
            ["echo"] = new Keyword((ForwardParser p, ParserState s, out Statement c) =>
                p.TryCreateEcho(s, out c), true),
            ["for"] = new Keyword((ForwardParser p, ParserState s, out Statement c) =>
                p.TryCreateFor(s, out c), true),
            ["if"] = new Keyword((ForwardParser p, ParserState s, out Statement c) =>
                p.TryCreateIfThen(s, out c), true),
            ["return"] = new Keyword((ForwardParser p, ParserState s, out Statement c) =>
                p.TryCreateReturn(s, out c), false),
            ["set"] = new Keyword((ForwardParser p, ParserState s, out Statement c) =>
                p.TryCreateSet(s, out c), true),
            ["unwrap"] = new Keyword((ForwardParser p, ParserState s, out Statement c) =>
                p.TryCreateUnwrap(s, out c), true),
            ["while"] = new Keyword((ForwardParser p, ParserState s, out Statement c) =>
                p.TryCreateWhile(s, out c), true),
            ["wrap"] = new Keyword((ForwardParser p, ParserState s, out Statement c) =>
                p.TryCreateWrap(s, out c), true)
        };

        private readonly Lexer _lexer;
        private readonly Func<string, string> _trimmer;

        public ForwardParser(string blockBegin, string blockContinue, string blockEnd, char escape,
            Func<string, string> trimmer)
        {
            _lexer = new Lexer(blockBegin, blockContinue, blockEnd, escape);
            _trimmer = trimmer;
        }

        public bool Parse(TextReader reader, ParserState state, out Statement statement)
        {
            _lexer.Reset(reader);
            _lexer.NextRaw();

            if (!TryParseStatement(state, out statement))
                return false;

            if (_lexer.Current.Type != LexemType.EndOfFile)
            {
                state.AddReport(CreateReportExpected("end of file"));

                return false;
            }

            return true;
        }

        private static Expression BuildInvoke(IFunction function, params Expression[] arguments)
        {
            var source = Expression.CreateConstant(Value.FromFunction(function));

            return Expression.CreateInvoke(source, arguments);
        }

        private bool TryCreateComment(out Statement statement)
        {
            do
            {
                _lexer.NextRaw();
            } while (_lexer.Current.Type == LexemType.Text);

            statement = Statement.NoOp;

            return true;
        }

        private bool TryCreateDeclare(ParserState state, out Statement statement)
        {
            return TryParseAssignment(state, StoreMode.Local, out statement);
        }

        private bool TryCreateDefine(ParserState state, out Statement statement)
        {
            state.AddReport(CreateReportObsolete("keyword \"define\"", "keyword \"declare\""));

            return TryParseAssignment(state, StoreMode.Global, out statement);
        }

        private bool TryCreateDump(ParserState state, out Statement statement)
        {
            return TryParseStatementOperand(state, Statement.CreateDump, out statement);
        }

        private bool TryCreateEcho(ParserState state, out Statement statement)
        {
            return TryParseStatementOperand(state, Statement.CreateEcho, out statement);
        }

        private bool TryCreateFor(ParserState state, out Statement statement)
        {
            if (!TryParseSymbol(state, out var element))
            {
                statement = Statement.NoOp;

                return false;
            }

            string key;
            string value;

            if (_lexer.Current.Type == LexemType.Comma)
            {
                _lexer.NextBlock();

                if (!TryParseSymbol(state, out value))
                {
                    statement = Statement.NoOp;

                    return false;
                }

                key = element;
            }
            else
            {
                key = string.Empty;
                value = element;
            }

            if (!TryParseExpected(state, LexemType.Symbol, "in", "'in' keyword") ||
                !TryParseExpression(state, out var source) ||
                !TryParseStatementBody(state, out var body))
            {
                statement = Statement.NoOp;

                return false;
            }

            Statement empty;

            if (_lexer.Current.Type == LexemType.BlockContinue)
            {
                _lexer.NextBlock();

                if (!TryParseExpected(state, LexemType.Symbol, "empty", "'empty' keyword") ||
                    !TryParseStatementBody(state, out empty))
                {
                    statement = Statement.NoOp;

                    return false;
                }
            }
            else
                empty = Statement.NoOp;

            statement = Statement.CreateFor(key, value, source, body, empty);

            return true;
        }

        private bool TryCreateIfThen(ParserState state, out Statement statement)
        {
            if (!TryParseExpression(state, out var ifCondition) ||
                !TryParseStatementBody(state, out var ifBody))
            {
                statement = Statement.NoOp;

                return false;
            }

            var branches = new List<(Expression, Statement)>();
            var final = Statement.NoOp;
            var next = true;

            branches.Add((ifCondition, ifBody));

            while (next && _lexer.Current.Type == LexemType.BlockContinue)
            {
                _lexer.NextBlock();

                switch (_lexer.Current.Type == LexemType.Symbol ? _lexer.Current.Value : string.Empty)
                {
                    case "elif":
                        _lexer.NextBlock();

                        if (!TryParseExpression(state, out var elifCondition) ||
                            !TryParseStatementBody(state, out var elifBody))
                        {
                            statement = Statement.NoOp;

                            return false;
                        }

                        branches.Add((elifCondition, elifBody));

                        break;

                    case "else":
                        _lexer.NextBlock();

                        if (!TryParseStatementBody(state, out var elseBody))
                        {
                            statement = Statement.NoOp;

                            return false;
                        }

                        final = elseBody;
                        next = false;

                        break;

                    default:
                        state.AddReport(CreateReportExpected("'elif' or 'else' keyword"));

                        statement = Statement.NoOp;

                        return false;
                }
            }

            for (var i = branches.Count - 1; i >= 0; --i)
                final = Statement.CreateIf(branches[i].Item1, branches[i].Item2, final);

            statement = final;

            return true;
        }

        private bool TryCreateReturn(ParserState state, out Statement statement)
        {
            return TryParseStatementOperand(state, Statement.CreateReturn, out statement);
        }

        private bool TryCreateSet(ParserState state, out Statement statement)
        {
            return TryParseAssignment(state, StoreMode.Global, out statement);
        }

        private bool TryCreateUnwrap(ParserState state, out Statement statement)
        {
            if (!TryParseStatementBody(state, out var body))
            {
                statement = Statement.NoOp;

                return false;
            }

            statement = Statement.CreateUnwrap(body);

            return true;
        }

        private bool TryCreateWhile(ParserState state, out Statement statement)
        {
            if (!TryParseExpression(state, out var condition) ||
                !TryParseStatementBody(state, out var body))
            {
                statement = Statement.NoOp;

                return false;
            }

            statement = Statement.CreateWhile(condition, body);

            return true;
        }

        private bool TryCreateWrap(ParserState state, out Statement statement)
        {
            if (!TryParseExpression(state, out var wrapper) ||
                !TryParseStatementBody(state, out var body))
            {
                statement = Statement.NoOp;

                return false;
            }

            statement = Statement.CreateWrap(wrapper, body);

            return true;
        }

        private bool TryParseAssignment(ParserState state, StoreMode mode, out Statement statement)
        {
            List<string>? arguments;

            if (!TryParseSymbol(state, out var name))
            {
                statement = Statement.NoOp;

                return false;
            }

            // Parse function arguments if provided
            if (_lexer.Current.Type == LexemType.ParenthesisBegin)
            {
                arguments = new List<string>();

                for (_lexer.NextBlock(); _lexer.Current.Type != LexemType.ParenthesisEnd;)
                {
                    if (!TryParseSymbol(state, out var symbol))
                    {
                        statement = Statement.NoOp;

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
                if (arguments is not null)
                {
                    statement = Statement.CreateAssignFunction(name, arguments, mode, Statement.NoOp);

                    return true;
                }

                // Arguments where not defined, build value assignment
                statement = Statement.CreateAssignValue(name, mode, Expression.Void);

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
                    if (!TryParseExpected(state, LexemType.Symbol, "to", "'to' keyword"))
                    {
                        statement = Statement.NoOp;

                        return false;
                    }
                }
            }
            else if (!TryParseExpected(state, LexemType.Symbol, "as", "'as' keyword"))
            {
                statement = Statement.NoOp;

                return false;
            }

            // Arguments were defined, build function assignment
            if (arguments is not null)
            {
                if (!TryParseStatementBody(state, out var body))
                {
                    statement = Statement.NoOp;

                    return false;
                }

                statement = Statement.CreateAssignFunction(name, arguments, mode, body);

                return true;
            }

            // No arguments provided and literal body follows, build render assignment
            if (_lexer.Current.Type == LexemType.Colon)
            {
                if (!TryParseStatementBody(state, out var body))
                {
                    statement = Statement.NoOp;

                    return false;
                }

                statement = Statement.CreateAssignRender(name, mode, body);

                return true;
            }

            // No arguments and no literal body, build value assignment
            if (!TryParseExpression(state, out var operand))
            {
                statement = Statement.NoOp;

                return false;
            }

            _lexer.NextRaw();

            statement = Statement.CreateAssignValue(name, mode, operand);

            return true;
        }

        private bool TryParseExpected(ParserState state, LexemType type, string value, string message)
        {
            if (_lexer.Current.Type != type || _lexer.Current.Value != value)
            {
                state.AddReport(CreateReportExpected(message));

                return false;
            }

            _lexer.NextBlock();

            return true;
        }

        private bool TryParseExpression(ParserState state, out Expression expression)
        {
            if (TryParseValue(state, out var operand))
                return TryParseExpressionOperand(state, operand, out expression);

            expression = Expression.Void;

            return false;
        }

        private bool TryParseExpressionOperand(ParserState state, Expression head, out Expression expression)
        {
            var operands = new Stack<Expression>();
            var operators = new Stack<Operator>();

            operands.Push(head);

            while (true)
            {
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

                if (!TryParseValue(state, out var operand))
                {
                    expression = Expression.Void;

                    return false;
                }

                operands.Push(operand);
            }
        }

        private bool TryParseKeyword(out Keyword keyword)
        {
            if (_lexer.Current.Type == LexemType.Symbol &&
                ForwardParser.Keywords.TryGetValue(_lexer.Current.Value, out keyword))
            {
                _lexer.NextBlock();

                return true;
            }

            keyword = default;

            return false;
        }

        private bool TryParseStatement(ParserState state, out Statement statement)
        {
            var statements = new List<Statement>();

            while
            (
                _lexer.Current.Type != LexemType.BlockContinue &&
                _lexer.Current.Type != LexemType.BlockEnd &&
                _lexer.Current.Type != LexemType.EndOfFile
            )
            {
                // Parse next statement or exit loop
                switch (_lexer.Current.Type)
                {
                    case LexemType.BlockBegin:
                        _lexer.NextBlock();

                        var blockLexem = _lexer.Current;
                        KeywordParser blockParse;

                        // Case 1: first block lexem is not a keyword, consider it as an implicit "echo" command
                        if (!TryParseKeyword(out var keyword))
                            blockParse = (ForwardParser p, ParserState s, out Statement c) => p.TryCreateEcho(s, out c);

                        // Case 2: first block lexem is a keyword but is missing mandatory operand, consider it as a
                        // symbol and parse as an implicit "echo" command
                        else if (keyword.HasMandatoryOperand && _lexer.Current.Type == LexemType.None)
                        {
                            var symbol = Expression.CreateSymbol(blockLexem.Value);

                            blockParse = (ForwardParser p, ParserState s, out Statement c) =>
                            {
                                if (!TryParseExpressionOperand(state, symbol, out var operand))
                                {
                                    c = Statement.NoOp;

                                    return false;
                                }

                                _lexer.NextRaw();

                                c = Statement.CreateEcho(operand);

                                return true;
                            };
                        }

                        // Case 3: first block lexem is a keyword with acceptable syntax, parse command accordingly
                        else
                            blockParse = keyword.Parse;

                        // Use delegate defined above to parse recognized command
                        if (!blockParse(this, state, out var blockStatement))
                        {
                            statement = Statement.NoOp;

                            return false;
                        }

                        if (_lexer.Current.Type != LexemType.BlockEnd)
                        {
                            state.AddReport(CreateReportExpected("end of block"));

                            statement = Statement.NoOp;

                            return false;
                        }

                        // Ignore empty statements
                        if (blockStatement is not null)
                            statements.Add(blockStatement);

                        _lexer.NextRaw();

                        break;

                    case LexemType.Text:
                        statements.Add(Statement.CreateLiteral(_trimmer(_lexer.Current.Value)));

                        _lexer.NextRaw();

                        break;

                    default:
                        state.AddReport(CreateReportExpected("text or block begin ('{')"));

                        statement = Statement.NoOp;

                        return false;
                }
            }

            if (statements.Count < 1)
                statement = Statement.NoOp;
            else if (statements.Count == 1)
                statement = statements[0];
            else
            {
                var composite = statements[statements.Count - 1];

                for (var i = statements.Count - 2; i >= 0; --i)
                    composite = Statement.CreateComposite(statements[i], composite);

                statement = composite;
            }

            return true;
        }

        private bool TryParseStatementBody(ParserState state, out Statement statement)
        {
            if (_lexer.Current.Type != LexemType.Colon)
            {
                state.AddReport(CreateReportExpected("body separator (':')"));

                statement = Statement.NoOp;

                return false;
            }

            _lexer.NextRaw();

            return TryParseStatement(state, out statement);
        }

        private bool TryParseStatementOperand(ParserState state, Func<Expression, Statement> constructor, out Statement statement)
        {
            if (!TryParseExpression(state, out var operand))
            {
                statement = Statement.NoOp;

                return false;
            }

            _lexer.NextRaw();

            statement = constructor(operand);

            return true;
        }

        private bool TryParseSymbol(ParserState state, out string name)
        {
            if (_lexer.Current.Type != LexemType.Symbol)
            {
                state.AddReport(CreateReportExpected("symbol (variable name)"));

                name = string.Empty;

                return false;
            }

            name = _lexer.Current.Value;

            _lexer.NextBlock();

            return true;
        }

        private bool TryParseValue(ParserState state, out Expression expression)
        {
            switch (_lexer.Current.Type)
            {
                case LexemType.Bang:
                    _lexer.NextBlock();

                    if (!TryParseValue(state, out var notExpression))
                    {
                        expression = Expression.Void;

                        return false;
                    }

                    expression = ForwardParser.BuildInvoke(BuiltinOperators.OperatorNot, notExpression);

                    return true;

                case LexemType.BracketBegin:
                    var elements = new List<ExpressionElement>();
                    var index = 0;

                    for (_lexer.NextBlock(); _lexer.Current.Type != LexemType.BracketEnd;)
                    {
                        if (!TryParseExpression(state, out var element))
                        {
                            expression = Expression.Void;

                            return false;
                        }

                        Expression key;
                        Expression value;

                        if (_lexer.Current.Type == LexemType.Colon)
                        {
                            _lexer.NextBlock();

                            if (!TryParseExpression(state, out value))
                            {
                                expression = Expression.Void;

                                return false;
                            }

                            if (element.Type == ExpressionType.Constant && element.Value.Type == ValueContent.Number &&
                                Math.Abs(element.Value.AsNumber - index) < double.Epsilon)
                            {
                                ++index;
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

                    if (!TryParseValue(state, out var minusRhs))
                    {
                        expression = Expression.Void;

                        return false;
                    }

                    var minusLhs = Expression.CreateConstant(0);

                    expression = ForwardParser.BuildInvoke(BuiltinOperators.OperatorSub, minusLhs, minusRhs);

                    return true;

                case LexemType.Number:
                    if (!double.TryParse(_lexer.Current.Value, NumberStyles.Number, CultureInfo.InvariantCulture,
                        out var number))
                        number = 0;

                    expression = Expression.CreateConstant(number);

                    _lexer.NextBlock();

                    break;

                case LexemType.ParenthesisBegin:
                    _lexer.NextBlock();

                    if (!TryParseExpression(state, out expression))
                        return false;

                    if (_lexer.Current.Type != LexemType.ParenthesisEnd)
                    {
                        state.AddReport(CreateReportExpected("parenthesis end (')')"));

                        return false;
                    }

                    _lexer.NextBlock();

                    return true;

                case LexemType.Plus:
                    _lexer.NextBlock();

                    return TryParseValue(state, out expression);

                case LexemType.String:
                    expression = Expression.CreateConstant(_lexer.Current.Value);

                    _lexer.NextBlock();

                    break;

                case LexemType.Symbol:
                    expression = Expression.CreateSymbol(_lexer.Current.Value);

                    _lexer.NextBlock();

                    break;

                default:
                    state.AddReport(CreateReportExpected("expression"));

                    expression = Expression.Void;

                    return false;
            }

            while (true)
            {
                switch (_lexer.Current.Type)
                {
                    case LexemType.BracketBegin:
                        _lexer.NextBlock();

                        if (!TryParseExpression(state, out var subscript))
                            return false;

                        if (_lexer.Current.Type != LexemType.BracketEnd)
                        {
                            state.AddReport(CreateReportExpected("array index end (']')"));

                            return false;
                        }

                        _lexer.NextBlock();

                        expression = Expression.CreateAccess(expression, subscript);

                        break;

                    case LexemType.Dot:
                        _lexer.NextBlock();

                        if (_lexer.Current.Type != LexemType.Symbol)
                        {
                            state.AddReport(CreateReportExpected("field name"));

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
                            if (!TryParseExpression(state, out var argument))
                                return false;

                            arguments.Add(argument);

                            if (_lexer.Current.Type == LexemType.Comma)
                                _lexer.NextBlock();
                        }

                        _lexer.NextBlock();

                        expression = Expression.CreateInvoke(expression, arguments);

                        break;

                    default:
                        return true;
                }
            }
        }

        private DocumentReport CreateReportExpected(string expected)
        {
            var current = _lexer.Current;
            var message = $"expected {expected}, found {current.Value}";

            return new DocumentReport(DocumentSeverity.Error, current.Offset, current.Length, message);
        }

        private DocumentReport CreateReportObsolete(string obsolete, string replacement)
        {
            var current = _lexer.Current;
            var message = $"{obsolete} is obsolete, please replace with {replacement}";

            return new DocumentReport(DocumentSeverity.Notice, current.Offset, current.Length, message);
        }
    }
}