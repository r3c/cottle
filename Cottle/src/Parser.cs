using System.Collections.Generic;
using System.Globalization;
using System.IO;

using Cottle.Exceptions;
using Cottle.Expressions;
using Cottle.Nodes;

namespace   Cottle
{
    class   Parser
    {
        #region Attributes / Instance

        private Lexer   lexer;

        #endregion

        #region Attributes / Static

        private static readonly Dictionary<string, Keyword> keywords = new Dictionary<string, Keyword>
        {
            {"_",       p => p.ParseKeywordComment ()},
            {"define",  p => p.ParseKeywordDefine ()},
            {"dump",    p => p.ParseKeywordDump ()},
            {"echo",    p => p.ParseKeywordEcho ()},
            {"for",     p => p.ParseKeywordFor ()},
            {"if",      p => p.ParseKeywordIf ()},
            {"return",  p => p.ParseKeywordReturn ()},
            {"set",     p => p.ParseKeywordSet ()},
            {"while",   p => p.ParseKeywordWhile ()}
        };

        #endregion

        #region Constructors

        public  Parser (LexerConfig config)
        {
            this.lexer = new Lexer (config);
        }

        #endregion

        #region Methods / Public

        public INode    Parse (TextReader reader)
        {
            INode   node;

            this.lexer.Reset (reader);
            this.lexer.Next (LexerMode.RAW);

            node = this.ParseRaw ();

            if (this.lexer.Current.Type != LexemType.EndOfFile)
                throw new UnexpectedException (this.lexer, "end of file");

            return node;
        }

        #endregion

        #region Methods / Private

        private INode   ParseBlock ()
        {
            Keyword keyword;
            INode   node;

            if (this.lexer.Current.Type == LexemType.Literal && Parser.keywords.TryGetValue (this.lexer.Current.Content, out keyword))
                this.lexer.Next (LexerMode.BLOCK);
            else
                keyword = p => p.ParseKeywordEcho ();

            node = keyword (this);

            if (this.lexer.Current.Type != LexemType.BraceEnd)
                throw new UnexpectedException (this.lexer, "end of block");

            this.lexer.Next (LexerMode.RAW);

            return node;
        }

        private INode   ParseBody ()
        {
            if (this.lexer.Current.Type != LexemType.Colon)
                throw new UnexpectedException (this.lexer, "body separator (':')");

            this.lexer.Next (LexerMode.RAW);

            return this.ParseRaw ();
        }

        private IExpression ParseExpression ()
        {
            List<IExpression>                               arguments;
            List<KeyValuePair<IExpression, IExpression>>    elements;
            IExpression                                     expression;
            int                                             index;
            IExpression                                     key;
            decimal                                         number;
            IExpression                                     value;

            switch (this.lexer.Current.Type)
            {
                case LexemType.BracketBegin:
                    elements = new List<KeyValuePair<IExpression, IExpression>> ();
                    index = 0;

                    for (this.lexer.Next (LexerMode.BLOCK); this.lexer.Current.Type != LexemType.BracketEnd; )
                    {
                        key = this.ParseExpression ();

                        if (this.lexer.Current.Type == LexemType.Colon)
                        {
                            this.lexer.Next (LexerMode.BLOCK);

                            value = this.ParseExpression ();
                        }
                        else
                        {
                            value = key;
                            key = new NumberExpression (index++);
                        }

                        elements.Add (new KeyValuePair<IExpression, IExpression> (key, value));

                        if (this.lexer.Current.Type == LexemType.Comma)
                            this.lexer.Next (LexerMode.BLOCK);
                    }

                    this.lexer.Next (LexerMode.BLOCK);

                    expression = new ArrayExpression (elements);

                    break;

                case LexemType.Number:
                    expression = new NumberExpression (decimal.TryParse (this.lexer.Current.Content, NumberStyles.Number, CultureInfo.InvariantCulture, out number) ? number : 0);

                    this.lexer.Next (LexerMode.BLOCK);

                    break;

                case LexemType.String:
                    expression = new StringExpression (this.lexer.Current.Content);

                    this.lexer.Next (LexerMode.BLOCK);

                    break;

                case LexemType.Literal:
                    expression = this.ParseName ();

                    break;

                default:
                    throw new UnexpectedException (this.lexer, "expression");
            }

            while (true)
            {
                switch (this.lexer.Current.Type)
                {
                    case LexemType.BracketBegin:
                        this.lexer.Next (LexerMode.BLOCK);

                        value = this.ParseExpression ();

                        if (this.lexer.Current.Type != LexemType.BracketEnd)
                            throw new UnexpectedException (this.lexer, "array index end (']')");

                        this.lexer.Next (LexerMode.BLOCK);

                        expression = new AccessExpression (expression, value);

                        break;

                    case LexemType.Dot:
                        this.lexer.Next (LexerMode.BLOCK);

                        if (this.lexer.Current.Type != LexemType.Literal)
                            throw new UnexpectedException (this.lexer, "field name");

                        expression = new AccessExpression (expression, new StringExpression (this.lexer.Current.Content));

                        this.lexer.Next (LexerMode.BLOCK);

                        break;

                    case LexemType.ParenthesisBegin:
                        arguments = new List<IExpression> ();

                        for (this.lexer.Next (LexerMode.BLOCK); this.lexer.Current.Type != LexemType.ParenthesisEnd; )
                        {
                            arguments.Add (this.ParseExpression ());

                            if (this.lexer.Current.Type == LexemType.Comma)
                                this.lexer.Next (LexerMode.BLOCK);
                        }

                        this.lexer.Next (LexerMode.BLOCK);

                        expression = new CallExpression (expression, arguments);

                        break;

                    default:
                        return expression;
                }
            }
        }

        private INode   ParseKeywordComment ()
        {
            do
            {
                this.lexer.Next (LexerMode.RAW);
            }
            while (this.lexer.Current.Type == LexemType.Text);

            return null;
        }

        private INode   ParseKeywordDefine ()
        {
            List<NameExpression>    arguments;
            ScopeMode               mode;
            NameExpression          name;

            name = this.ParseName ();

            if (this.lexer.Current.Type != LexemType.ParenthesisBegin)
                throw new UnexpectedException (this.lexer, "arguments begin ('(')");

            arguments = new List<NameExpression> ();

            for (this.lexer.Next (LexerMode.BLOCK); this.lexer.Current.Type != LexemType.ParenthesisEnd; )
            {
                arguments.Add (this.ParseName ());

                if (this.lexer.Current.Type == LexemType.Comma)
                    this.lexer.Next (LexerMode.BLOCK);
            }

            this.lexer.Next (LexerMode.BLOCK);

            switch (this.lexer.Current.Type == LexemType.Literal ? this.lexer.Current.Content : string.Empty)
            {
                case "as":
                    this.lexer.Next (LexerMode.BLOCK);

                    mode = ScopeMode.Local;

                    break;

                case "to":
                    this.lexer.Next (LexerMode.BLOCK);

                    mode = ScopeMode.Closest;

                    break;

                default:
                    throw new UnexpectedException (this.lexer, "'as' or 'to' keyword");
            }

            return new DefineNode (name, arguments, this.ParseBody (), mode);
        }

        private INode   ParseKeywordDump ()
        {
            return new DumpNode (this.ParseStatement ());
        }

        private INode   ParseKeywordEcho ()
        {
            return new EchoNode (this.ParseStatement ());
        }

        private INode   ParseKeywordFor ()
        {
            INode           body;
            INode           empty;
            IExpression     from;
            NameExpression  key;
            NameExpression  value;

            key = this.ParseName ();

            if (this.lexer.Current.Type == LexemType.Comma)
            {
                this.lexer.Next (LexerMode.BLOCK);

                value = this.ParseName ();
            }
            else
            {
                value = key;
                key = null;
            }

            this.ParseSingle (LexemType.Literal, "in", "'in' keyword");

            from = this.ParseExpression ();
            body = this.ParseBody ();

            if (this.lexer.Current.Type == LexemType.Pipe)
            {
                this.lexer.Next (LexerMode.BLOCK);

                this.ParseSingle (LexemType.Literal, "empty", "'empty' keyword");

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

            while (fallback == null && this.lexer.Current.Type == LexemType.Pipe)
            {
                this.lexer.Next (LexerMode.BLOCK);

                switch (this.lexer.Current.Type == LexemType.Literal ? this.lexer.Current.Content : string.Empty)
                {
                    case "elif":
                        this.lexer.Next (LexerMode.BLOCK);

                        test = this.ParseExpression ();

                        branches.Add (new IfNode.Branch (test, this.ParseBody ()));

                        break;

                    case "else":
                        this.lexer.Next (LexerMode.BLOCK);

                        fallback = this.ParseBody ();

                        break;

                    default:
                        throw new UnexpectedException (this.lexer, "'elif' or 'else' keyword");
                }
            }

            return new IfNode (branches, fallback);
        }

        private INode   ParseKeywordReturn ()
        {
            return new ReturnNode (this.ParseStatement ());
        }

        private INode   ParseKeywordSet ()
        {
            ScopeMode       mode;
            NameExpression  name;

            name = this.ParseName ();

            switch (this.lexer.Current.Type == LexemType.Literal ? this.lexer.Current.Content : string.Empty)
            {
                case "as":
                    this.lexer.Next (LexerMode.BLOCK);

                    mode = ScopeMode.Local;

                    break;

                case "to":
                    this.lexer.Next (LexerMode.BLOCK);

                    mode = ScopeMode.Closest;

                    break;

                default:
                    throw new UnexpectedException (this.lexer, "'as' or 'to' keyword");
            }

            return new SetNode (name, this.ParseStatement (), mode);
        }

        private INode   ParseKeywordWhile ()
        {
            IExpression test = this.ParseExpression ();

            return new WhileNode (test, this.ParseBody ());
        }

        private NameExpression  ParseName ()
        {
            NameExpression  name;

            if (this.lexer.Current.Type != LexemType.Literal)
                throw new UnexpectedException (this.lexer, "variable name");

            name = new NameExpression (this.lexer.Current.Content);

            this.lexer.Next (LexerMode.BLOCK);

            return name;
        }

        private INode   ParseRaw ()
        {
            List<INode> nodes;
            INode       node;

            nodes = new List<INode> ();

            while (true)
            {
                switch (this.lexer.Current.Type)
                {
                    case LexemType.BraceBegin:
                        this.lexer.Next (LexerMode.BLOCK);

                        node = this.ParseBlock ();

                        if (node != null)
                            nodes.Add (node);

                        break;

                    case LexemType.BraceEnd:
                    case LexemType.EndOfFile:
                    case LexemType.Pipe:
                        return nodes.Count != 1 ? new CompositeNode (nodes) : nodes[0];

                    case LexemType.Text:
                        if (!string.IsNullOrEmpty (this.lexer.Current.Content))
                            nodes.Add (new TextNode (this.lexer.Current.Content));

                        this.lexer.Next (LexerMode.RAW);

                        break;

                    default:
                        throw new UnexpectedException (this.lexer, "text or block begin ('{')");
                }
            }
        }

        private void    ParseSingle (LexemType type, string value, string expected)
        {
            if (this.lexer.Current.Type != type || this.lexer.Current.Content != value)
                throw new UnexpectedException (this.lexer, expected);

            this.lexer.Next (LexerMode.BLOCK);
        }

        private IExpression ParseStatement ()
        {
            IExpression expression;

            expression = this.ParseExpression ();

            this.lexer.Next (LexerMode.RAW);

            return expression;
        }

        #endregion

        #region Types

        private delegate INode  Keyword (Parser parser);

        #endregion
    }
}
