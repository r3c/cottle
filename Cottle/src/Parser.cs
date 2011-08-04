using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Exceptions;
using Cottle.Expressions;
using Cottle.Lexers;
using Cottle.Nodes;
using Cottle.Values;

namespace   Cottle
{
    public class    Parser
    {
        #region Constants

        private static readonly Dictionary<string, Keyword> KEYWORDS = new Dictionary<string, Keyword>
        {
            {"echo",    p => p.ParseKeywordEcho ()},
            {"for",     p => p.ParseKeywordFor ()},
            {"if",      p => p.ParseKeywordIf ()},
            {"set",     p => p.ParseKeywordSet ()},
            {"while",   p => p.ParseKeywordWhile ()}
        };

        #endregion

        #region Attributes

        private Lexer   lexer = new Lexer ();

        #endregion

        #region Methods

        public Document Parse (TextReader reader)
        {
            INode   root;

            this.lexer.Initialize (reader);
            this.lexer.Mode (Lexer.LexerMode.RAW);
            this.lexer.Next ();

            root = this.ParseRaw ();

            if (this.lexer.Type != Lexer.LexemType.EOF)
                throw new UnexpectedException (this.lexer, "end of file");

            return new Document (root);
        }

        private INode   ParseBlock ()
        {
            Keyword keyword;
            INode   node;

            if (this.lexer.Type == Lexer.LexemType.LITERAL && Parser.KEYWORDS.TryGetValue (this.lexer.Value, out keyword))
                this.lexer.Next ();
            else
                keyword = p => p.ParseKeywordEcho ();

            node = keyword (this);

            if (this.lexer.Type != Lexer.LexemType.BRACE_END)
                throw new UnexpectedException (this.lexer, "end block character ('}')");

            this.lexer.Mode (Lexer.LexerMode.RAW);
            this.lexer.Next ();

            return node;
        }

        private INode   ParseBody ()
        {
            if (this.lexer.Type != Lexer.LexemType.COLON)
                throw new UnexpectedException (this.lexer, "body separator (':')");

            this.lexer.Mode (Lexer.LexerMode.RAW);
            this.lexer.Next ();

            return this.ParseRaw ();
        }

        private IExpression ParseExpression ()
        {
            List<IExpression>   arguments;
            List<NameExpression> fields;
            NameExpression       variable;

            switch (this.lexer.Type)
            {
                case Lexer.LexemType.BRACKET_BEGIN:
                case Lexer.LexemType.NUMBER:
                case Lexer.LexemType.STRING:
                    return new ConstantExpression (this.ParseValue ());

                case Lexer.LexemType.LITERAL:
                    variable = this.ParseName ();

                    if (this.lexer.Type == Lexer.LexemType.PARENTHESIS_BEGIN)
                    {
                        arguments = new List<IExpression> ();

                        for (this.lexer.Next (); this.lexer.Type != Lexer.LexemType.PARENTHESIS_END; )
                        {
                            arguments.Add (this.ParseExpression ());

                            if (this.lexer.Type == Lexer.LexemType.COMMA)
                                this.lexer.Next ();
                        }

                        this.lexer.Next ();

                        return new FunctionExpression (variable, arguments);
                    }

                    fields = new List<NameExpression> ();
                    fields.Add (variable);

                    while (this.lexer.Type == Lexer.LexemType.FIELD)
                    {
                        this.lexer.Next ();

                        fields.Add (this.ParseName ());
                    }

                    return new AccessExpression (fields);

                default:
                    throw new UnexpectedException (this.lexer, "expression");
            }
        }

        private INode   ParseKeywordEcho ()
        {
            return new EchoNode (this.ParseExpression ());
        }

        private INode   ParseKeywordFor ()
        {
            INode           body;
            INode           empty;
            NameExpression   first;
            IExpression     from;
            NameExpression   key;
            NameExpression   value;

            first = this.ParseName ();

            if (this.lexer.Type == Lexer.LexemType.COMMA)
            {
                this.lexer.Next ();

                key = first;
                value = this.ParseName ();
            }
            else
            {
                key = null;
                value = first;
            }

            this.ParseUnused (Lexer.LexemType.LITERAL, "in", "'in' keyword");

            from = this.ParseExpression ();
            body = this.ParseBody ();

            if (this.lexer.Type == Lexer.LexemType.PIPE)
            {
                this.lexer.Next ();

                this.ParseUnused (Lexer.LexemType.LITERAL, "empty", "'empty' keyword");

                empty = this.ParseBody ();
            }
            else
                empty = null;

            return new ForNode (from, key, value, body, empty);
        }

        private INode   ParseKeywordIf ()
        {
            List<IfNode.Branch> branches = new List<IfNode.Branch> ();
            INode               fallback = null;
            IExpression         test;

            test = this.ParseExpression ();

            branches.Add (new IfNode.Branch (test, this.ParseBody ()));

            while (fallback == null && this.lexer.Type == Lexer.LexemType.PIPE)
            {
                this.lexer.Next ();

                switch (this.lexer.Type == Lexer.LexemType.LITERAL ? this.lexer.Value : string.Empty)
                {
                    case "elif":
                        this.lexer.Next ();

                        test = this.ParseExpression ();

                        branches.Add (new IfNode.Branch (test, this.ParseBody ()));

                        break;

                    case "else":
                        this.lexer.Next ();

                        fallback = this.ParseBody ();

                        break;

                    default:
                        throw new UnexpectedException (this.lexer, "'elif' or 'else' keyword");
                }
            }

            return new IfNode (branches, fallback);
        }

        private INode   ParseKeywordSet ()
        {
            NameExpression   alias;
            IExpression     value;

            alias = this.ParseName ();

            this.ParseUnused (Lexer.LexemType.LITERAL, "to", "'to' keyword");

            value = this.ParseExpression ();

            return new SetNode (alias, value);
        }

        private INode   ParseKeywordWhile ()
        {
            IExpression test;

            test = this.ParseExpression ();

            return new WhileNode (test, this.ParseBody ());
        }

        private NameExpression  ParseName ()
        {
            NameExpression  name;

            switch (this.lexer.Type)
            {
                case Lexer.LexemType.LITERAL:
                case Lexer.LexemType.NUMBER:
                    name = new NameExpression (this.lexer.Value);

                    this.lexer.Next ();

                    return name;

                default:
                    throw new UnexpectedException (this.lexer, "variable name");
            }
        }

        private INode   ParseRaw ()
        {
            bool        first;
            List<INode> nodes;
            string      value;

            first = true;
            nodes = new List<INode> ();

            while (true)
            {
                switch (this.lexer.Type)
                {
                    case Lexer.LexemType.BRACE_END:
                    case Lexer.LexemType.PIPE:
                    case Lexer.LexemType.EOF:
                        this.lexer.Mode (Lexer.LexerMode.BLOCK);

                        return nodes.Count != 1 ? new CompositeNode (nodes) : nodes[0];

                    case Lexer.LexemType.BRACE_BEGIN:
                        this.lexer.Mode (Lexer.LexerMode.BLOCK);
                        this.lexer.Next ();

                        nodes.Add (this.ParseBlock ());

                        break;

                    case Lexer.LexemType.LITERAL:
                    case Lexer.LexemType.STRING:
                        value = first && this.lexer.Type == Lexer.LexemType.LITERAL ?
                            this.lexer.Value.TrimStart () :
                            this.lexer.Value;

                        if (!string.IsNullOrEmpty (value))
                            nodes.Add (new RawNode (value));

                        this.lexer.Next ();

                        break;

                    default:
                        throw new UnexpectedException (this.lexer, "text or begin block character ('{')");
                }

                first = false;
            }
        }

        private void    ParseUnused (Lexer.LexemType type, string literal, string expected)
        {
            if (this.lexer.Type != type || this.lexer.Value != literal)
                throw new UnexpectedException (this.lexer, expected);

            this.lexer.Next ();
        }

        private IValue  ParseValue ()
        {
            List<KeyValuePair<IValue, IValue>>  collection;
            int                                 index;
            KeyValuePair<IValue, IValue>        item;
            decimal                             number;
            IValue                              value;

            switch (this.lexer.Type)
            {
                case Lexer.LexemType.BRACKET_BEGIN:
                    collection = new List<KeyValuePair<IValue, IValue>> ();
                    index = 0;

                    for (this.lexer.Next (); this.lexer.Type != Lexer.LexemType.BRACKET_END; )
                    {
                        value = this.ParseValue ();

                        if (this.lexer.Type == Lexer.LexemType.COLON)
                        {
                            this.lexer.Next ();

                            item = new KeyValuePair<IValue, IValue> (value, this.ParseValue ());
                        }
                        else
                            item = new KeyValuePair<IValue, IValue> (new NumberValue (index++), value);

                        collection.Add (item);

                        if (this.lexer.Type == Lexer.LexemType.COMMA)
                            this.lexer.Next ();
                    }

                    this.lexer.Next ();

                    return new ArrayValue (collection);

                case Lexer.LexemType.NUMBER:
                    value = new NumberValue (decimal.TryParse (this.lexer.Value, out number) ? number : default (decimal));

                    this.lexer.Next ();

                    return value;

                case Lexer.LexemType.STRING:
                    value = new StringValue (this.lexer.Value);

                    this.lexer.Next ();

                    return value;

                default:
                    throw new UnexpectedException (this.lexer, "constant expression");
            }
        }

        #endregion

        #region Types

        private delegate INode  Keyword (Parser parser);

        #endregion
    }
}
