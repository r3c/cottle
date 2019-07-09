using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Cottle.Builtins;
using Cottle.Exceptions;
using Cottle.Parsers.Forward;
using Cottle.Values;

namespace Cottle.Parsers
{
    internal class ForwardParser : IParser
    {
        private static readonly Dictionary<string, Func<ForwardParser, Command>> Keywords =
            new Dictionary<string, Func<ForwardParser, Command>>
            {
                { "_", p => p.ParseKeywordComment() },
                { "declare", p => p.ParseKeywordDeclare() },
                { "define", p => p.ParseKeywordSet() },
                { "dump", p => p.ParseKeywordOperand(CommandType.Dump) },
                { "echo", p => p.ParseKeywordOperand(CommandType.Echo) },
                { "for", p => p.ParseKeywordFor() },
                { "if", p => p.ParseKeywordIf() },
                { "return", p => p.ParseKeywordOperand(CommandType.Return) },
                { "set", p => p.ParseKeywordSet() },
                { "while", p => p.ParseKeywordWhile() }
            };

        private readonly Lexer _lexer;

        public ForwardParser(string blockBegin, string blockContinue, string blockEnd, char escape)
        {
            _lexer = new Lexer(blockBegin, blockContinue, blockEnd, escape);
        }

        public Command Parse(TextReader reader)
        {
            _lexer.Reset(reader);
            _lexer.NextRaw();

            var command = ParseCommand();

            if (_lexer.Current.Type != LexemType.EndOfFile)
                throw Raise("end of file");

            return command;
        }

        private Expression BuildOperator(IFunction function, params Expression[] arguments)
        {
            return new Expression
            {
                Arguments = arguments,
                Source = new Expression
                {
                    Type = ExpressionType.Constant,
                    Value = new FunctionValue(function)
                },
                Type = ExpressionType.Invoke
            };
        }

        private Command ParseAssignment(StoreMode mode)
        {
            List<string> arguments;
            var name = ParseSymbol();

            // Parse function arguments if provided
            if (_lexer.Current.Type == LexemType.ParenthesisBegin)
            {
                arguments = new List<string>();

                for (_lexer.NextBlock(); _lexer.Current.Type != LexemType.ParenthesisEnd;)
                {
                    arguments.Add(ParseSymbol());

                    if (_lexer.Current.Type == LexemType.Comma)
                        _lexer.NextBlock();
                }

                _lexer.NextBlock();
            }
            else
            {
                arguments = null;
            }

            // Early exit if no body, render nor value is defined
            if (_lexer.Current.Type != LexemType.Symbol)
            {
                _lexer.NextRaw();

                // Arguments were defined, build function assignment
                if (arguments != null)
                    return new Command
                    {
                        Arguments = arguments.ToArray(),
                        Mode = mode,
                        Name = name,
                        Type = CommandType.AssignFunction
                    };

                // Arguments where not defined, build value assignment
                return new Command
                {
                    Mode = mode,
                    Name = name,
                    Operand = Expression.Empty,
                    Type = CommandType.AssignValue
                };
            }

            // Parse 'as' or 'to' keyword
            if (mode == StoreMode.Global)
            {
                // <TODO> remove legacy keywords handling
                // FIXME: should raise event
                if (_lexer.Current.Content == "as")
                {
                    _lexer.NextBlock();

                    mode = StoreMode.Local;
                }
                else
                    // </TODO>
                {
                    ParseExpected(LexemType.Symbol, "to", "'to' keyword");
                }
            }
            else
            {
                ParseExpected(LexemType.Symbol, "as", "'as' keyword");
            }

            // Arguments were defined, build function assignment
            if (arguments != null)
                return new Command
                {
                    Arguments = arguments.ToArray(),
                    Body = ParseBody(),
                    Mode = mode,
                    Name = name,
                    Type = CommandType.AssignFunction
                };

            // No arguments provided and literal body follows, build render assignment
            if (_lexer.Current.Type == LexemType.Colon)
                return new Command
                {
                    Body = ParseBody(),
                    Mode = mode,
                    Name = name,
                    Type = CommandType.AssignRender
                };

            // No arguments and no literal body, build value assignment
            return new Command
            {
                Mode = mode,
                Name = name,
                Operand = ParseOperand(),
                Type = CommandType.AssignValue
            };
        }

        private Command ParseBody()
        {
            if (_lexer.Current.Type != LexemType.Colon)
                Raise("body separator (':')");

            _lexer.NextRaw();

            return ParseCommand();
        }

        private Command ParseCommand()
        {
            Command head = null;
            Command tail = null;

            while
            (
                _lexer.Current.Type != LexemType.BlockContinue &&
                _lexer.Current.Type != LexemType.BlockEnd &&
                _lexer.Current.Type != LexemType.EndOfFile
            )
            {
                // Parse next block or exit loop
                Command current;
                switch (_lexer.Current.Type)
                {
                    case LexemType.BlockBegin:
                        _lexer.NextBlock();

                        if (_lexer.Current.Type == LexemType.Symbol &&
                            ForwardParser.Keywords.TryGetValue(_lexer.Current.Content, out var parse))
                            _lexer.NextBlock();
                        else
                            parse = p => p.ParseKeywordOperand(CommandType.Echo);

                        current = parse(this);

                        if (_lexer.Current.Type != LexemType.BlockEnd)
                            throw Raise("end of block");

                        _lexer.NextRaw();

                        break;

                    case LexemType.Text:
                        current = new Command
                        {
                            Text = _lexer.Current.Content,
                            Type = CommandType.Literal
                        };

                        _lexer.NextRaw();

                        break;

                    default:
                        throw Raise("text or block begin ('{')");
                }

                // Ignore empty blocks
                if (current == null)
                    continue;

                // Chain current block to parent
                if (tail != null)
                {
                    tail.Next = new Command
                    {
                        Body = tail.Next,
                        Next = current,
                        Type = CommandType.Composite
                    };

                    tail = tail.Next;
                }
                else if (head != null)
                {
                    tail = new Command
                    {
                        Body = head,
                        Next = current,
                        Type = CommandType.Composite
                    };

                    head = tail;
                }
                else
                {
                    head = current;
                }
            }

            return head ?? new Command
            {
                Text = string.Empty,
                Type = CommandType.Literal
            };
        }

        private void ParseExpected(LexemType type, string value, string expected)
        {
            if (_lexer.Current.Type != type || _lexer.Current.Content != value)
                throw Raise(expected);

            _lexer.NextBlock();
        }

        private Expression ParseExpression()
        {
            var operands = new Stack<Expression>();
            var operators = new Stack<Operator>();

            while (true)
            {
                operands.Push(ParseValue());

                Operator current;
                Expression value;
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
                            current = operators.Pop();
                            value = operands.Pop();

                            operands.Push(BuildOperator(current.Function, operands.Pop(), value));
                        }

                        return operands.Pop();
                }

                _lexer.NextBlock();

                while (operators.Count > 0 && operators.Peek().Precedence >= current.Precedence)
                {
                    var other = operators.Pop();
                    value = operands.Pop();

                    operands.Push(BuildOperator(other.Function, operands.Pop(), value));
                }

                operators.Push(current);
            }
        }

        private Command ParseKeywordComment()
        {
            do
            {
                _lexer.NextRaw();
            } while (_lexer.Current.Type == LexemType.Text);

            return null;
        }

        private Command ParseKeywordDeclare()
        {
            return ParseAssignment(StoreMode.Local);
        }

        private Command ParseKeywordFor()
        {
            Command empty;
            string value;

            var key = ParseSymbol();

            if (_lexer.Current.Type == LexemType.Comma)
            {
                _lexer.NextBlock();

                value = ParseSymbol();
            }
            else
            {
                value = key;
                key = string.Empty;
            }

            ParseExpected(LexemType.Symbol, "in", "'in' keyword");

            var source = ParseExpression();
            var body = ParseBody();

            if (_lexer.Current.Type == LexemType.BlockContinue)
            {
                _lexer.NextBlock();

                ParseExpected(LexemType.Symbol, "empty", "'empty' keyword");

                empty = ParseBody();
            }
            else
            {
                empty = null;
            }

            return new Command
            {
                Body = body,
                Key = key,
                Name = value,
                Next = empty,
                Operand = source,
                Type = CommandType.For
            };
        }

        private Command ParseKeywordIf()
        {
            var condition = ParseExpression();
            var result = new Command
            {
                Body = ParseBody(),
                Operand = condition,
                Type = CommandType.If
            };

            var current = result;

            while (current.Next == null && _lexer.Current.Type == LexemType.BlockContinue)
            {
                _lexer.NextBlock();

                switch (_lexer.Current.Type == LexemType.Symbol ? _lexer.Current.Content : string.Empty)
                {
                    case "elif":
                        _lexer.NextBlock();

                        condition = ParseExpression();

                        current.Next = new Command
                        {
                            Body = ParseBody(),
                            Operand = condition,
                            Type = CommandType.If
                        };

                        current = current.Next;

                        break;

                    case "else":
                        _lexer.NextBlock();

                        current.Next = ParseBody();

                        break;

                    default:
                        throw Raise("'elif' or 'else' keyword");
                }
            }

            return result;
        }

        private Command ParseKeywordOperand(CommandType type)
        {
            return new Command
            {
                Operand = ParseOperand(),
                Type = type
            };
        }

        private Command ParseKeywordSet()
        {
            return ParseAssignment(StoreMode.Global);
        }

        private Command ParseKeywordWhile()
        {
            var condition = ParseExpression();
            var body = ParseBody();

            return new Command
            {
                Body = body,
                Operand = condition,
                Type = CommandType.While
            };
        }

        private Expression ParseOperand()
        {
            var expression = ParseExpression();

            _lexer.NextRaw();

            return expression;
        }

        private string ParseSymbol()
        {
            if (_lexer.Current.Type != LexemType.Symbol)
                throw Raise("symbol (variable name)");

            var name = _lexer.Current.Content;

            _lexer.NextBlock();

            return name;
        }

        private Expression ParseValue()
        {
            Expression expression;
            Expression value;

            switch (_lexer.Current.Type)
            {
                case LexemType.Bang:
                    _lexer.NextBlock();

                    return BuildOperator(BuiltinOperators.OperatorNot, ParseValue());

                case LexemType.BracketBegin:
                    var elements = new List<ExpressionElement>();
                    var index = 0;

                    for (_lexer.NextBlock(); _lexer.Current.Type != LexemType.BracketEnd;)
                    {
                        var key = ParseExpression();

                        if (_lexer.Current.Type == LexemType.Colon)
                        {
                            _lexer.NextBlock();

                            value = ParseExpression();
                        }
                        else
                        {
                            value = key;
                            key = new Expression
                            {
                                Type = ExpressionType.Constant,
                                Value = index++
                            };
                        }

                        elements.Add(new ExpressionElement(key, value));

                        if (_lexer.Current.Type == LexemType.Comma)
                            _lexer.NextBlock();
                    }

                    expression = new Expression
                    {
                        Elements = elements.ToArray(),
                        Type = ExpressionType.Map
                    };

                    _lexer.NextBlock();

                    break;

                case LexemType.Minus:
                    _lexer.NextBlock();

                    expression = new Expression
                    {
                        Type = ExpressionType.Constant,
                        Value = 0
                    };

                    return BuildOperator(BuiltinOperators.OperatorSub, expression, ParseValue());

                case LexemType.Number:
                    if (!decimal.TryParse(_lexer.Current.Content, NumberStyles.Number, CultureInfo.InvariantCulture,
                        out var number))
                        number = 0;

                    expression = new Expression
                    {
                        Type = ExpressionType.Constant,
                        Value = number
                    };

                    _lexer.NextBlock();

                    break;

                case LexemType.ParenthesisBegin:
                    _lexer.NextBlock();

                    expression = ParseExpression();

                    if (_lexer.Current.Type != LexemType.ParenthesisEnd)
                        throw Raise("parenthesis end (')')");

                    _lexer.NextBlock();

                    return expression;

                case LexemType.Plus:
                    _lexer.NextBlock();

                    return ParseValue();

                case LexemType.String:
                    expression = new Expression
                    {
                        Type = ExpressionType.Constant,
                        Value = _lexer.Current.Content
                    };

                    _lexer.NextBlock();

                    break;

                case LexemType.Symbol:
                    expression = new Expression
                    {
                        Type = ExpressionType.Symbol,
                        Value = _lexer.Current.Content
                    };

                    _lexer.NextBlock();

                    break;

                default:
                    throw Raise("expression");
            }

            while (true)
                switch (_lexer.Current.Type)
                {
                    case LexemType.BracketBegin:
                        _lexer.NextBlock();

                        value = ParseExpression();

                        if (_lexer.Current.Type != LexemType.BracketEnd)
                            throw Raise("array index end (']')");

                        _lexer.NextBlock();

                        expression = new Expression
                        {
                            Source = expression,
                            Subscript = value,
                            Type = ExpressionType.Access
                        };

                        break;

                    case LexemType.Dot:
                        _lexer.NextBlock();

                        if (_lexer.Current.Type != LexemType.Symbol)
                            throw Raise("field name");

                        expression = new Expression
                        {
                            Source = expression,
                            Subscript = new Expression
                            {
                                Type = ExpressionType.Constant,
                                Value = _lexer.Current.Content
                            },
                            Type = ExpressionType.Access
                        };

                        _lexer.NextBlock();

                        break;

                    case LexemType.ParenthesisBegin:
                        var arguments = new List<Expression>();

                        for (_lexer.NextBlock(); _lexer.Current.Type != LexemType.ParenthesisEnd;)
                        {
                            arguments.Add(ParseExpression());

                            if (_lexer.Current.Type == LexemType.Comma)
                                _lexer.NextBlock();
                        }

                        _lexer.NextBlock();

                        expression = new Expression
                        {
                            Arguments = arguments.ToArray(),
                            Source = expression,
                            Type = ExpressionType.Invoke
                        };

                        break;

                    default:
                        return expression;
                }
        }

        private Exception Raise(string expected)
        {
            return new ParseException(_lexer.Column, _lexer.Line, _lexer.Current.Content, expected);
        }
    }
}