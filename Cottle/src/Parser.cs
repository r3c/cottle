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

            if (this.lexer.Current.Type != Lexer.LexemType.EOF)
                throw new UnexpectedException (this.lexer, "end of file");

            return new Document (root);
        }

        private INode   ParseBlock ()
        {
            Keyword keyword;
            INode   node;

            if (this.lexer.Current.Type == Lexer.LexemType.LITERAL && Parser.KEYWORDS.TryGetValue (this.lexer.Current.Data, out keyword))
                this.lexer.Next ();
            else
                keyword = p => p.ParseKeywordEcho ();

            node = keyword (this);

            if (this.lexer.Current.Type != Lexer.LexemType.BRACE_END)
                throw new UnexpectedException (this.lexer, "end block character ('}')");

            this.lexer.Mode (Lexer.LexerMode.RAW);
            this.lexer.Next ();

            return node;
        }

        private INode   ParseBody ()
        {
            if (this.lexer.Current.Type != Lexer.LexemType.COLON)
                throw new UnexpectedException (this.lexer, "body separator (':')");

            this.lexer.Mode (Lexer.LexerMode.RAW);
            this.lexer.Next ();

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
                case Lexer.LexemType.BRACKET_BEGIN:
                    elements = new List<KeyValuePair<IExpression, IExpression>> ();
                    index = 0;

                    for (this.lexer.Next (); this.lexer.Current.Type != Lexer.LexemType.BRACKET_END; )
                    {
                        key = this.ParseExpression ();

                        if (this.lexer.Current.Type == Lexer.LexemType.COLON)
                        {
                            this.lexer.Next ();

                            value = this.ParseExpression ();
                        }
                        else
                        {
                            value = key;
                            key = new NumberExpression (index++);
                        }

                        elements.Add (new KeyValuePair<IExpression, IExpression> (key, value));

                        if (this.lexer.Current.Type == Lexer.LexemType.COMMA)
                            this.lexer.Next ();
                    }

                    this.lexer.Next ();

                    expression = new ArrayExpression (elements);

                    break;

                case Lexer.LexemType.NUMBER:
                    expression = new NumberExpression (decimal.TryParse (this.lexer.Current.Data, out number) ? number : 0);

                    this.lexer.Next ();

                    break;

                case Lexer.LexemType.STRING:
                    expression = new StringExpression (this.lexer.Current.Data);

                    this.lexer.Next ();

                    break;

                case Lexer.LexemType.LITERAL:
                    expression = this.ParseName ();

                    break;

                default:
                    throw new UnexpectedException (this.lexer, "expression");
            }

            while (true)
            {
                switch (this.lexer.Current.Type)
                {
                    case Lexer.LexemType.BRACKET_BEGIN:
                        this.lexer.Next ();

                        value = this.ParseExpression ();

                        if (this.lexer.Current.Type != Lexer.LexemType.BRACKET_END)
                            throw new UnexpectedException (this.lexer, "array index end (']')");

                        this.lexer.Next ();

                        expression = new AccessExpression (expression, value);

                        break;

                    case Lexer.LexemType.DOT:
                        this.lexer.Next ();

                        if (this.lexer.Current.Type != Lexer.LexemType.LITERAL)
                            throw new UnexpectedException (this.lexer, "field name");

                        expression = new AccessExpression (expression, new StringExpression (this.lexer.Current.Data));

                        this.lexer.Next ();

                        break;

                    case Lexer.LexemType.PARENTHESIS_BEGIN:
                        arguments = new List<IExpression> ();

                        for (this.lexer.Next (); this.lexer.Current.Type != Lexer.LexemType.PARENTHESIS_END; )
                        {
                            arguments.Add (this.ParseExpression ());

                            if (this.lexer.Current.Type == Lexer.LexemType.COMMA)
                                this.lexer.Next ();
                        }

                        this.lexer.Next ();

                        expression = new CallExpression (expression, arguments);

                        break;

                    default:
                        return expression;
                }
            }
        }

        private INode   ParseKeywordComment ()
        {
            this.ParseExpression ();

            return null;
        }

        private INode   ParseKeywordDefine ()
        {
            List<NameExpression>    arguments;
            NameExpression          name = this.ParseName ();
            Scope.SetMode           mode;

            if (this.lexer.Current.Type != Lexer.LexemType.PARENTHESIS_BEGIN)
                throw new UnexpectedException (this.lexer, "arguments begin ('(')");

            arguments = new List<NameExpression> ();

            for (this.lexer.Next (); this.lexer.Current.Type != Lexer.LexemType.PARENTHESIS_END; )
            {
                arguments.Add (this.ParseName ());

                if (this.lexer.Current.Type == Lexer.LexemType.COMMA)
                    this.lexer.Next ();
            }

            this.lexer.Next ();

            switch (this.lexer.Current.Type == Lexer.LexemType.LITERAL ? this.lexer.Current.Data : string.Empty)
            {
                case "as":
                    this.lexer.Next ();

                    mode = Scope.SetMode.LOCAL;

                    break;

                case "to":
                    this.lexer.Next ();

                    mode = Scope.SetMode.ANYWHERE;

                    break;

                default:
                    throw new UnexpectedException (this.lexer, "'as' or 'to' keyword");
            }

            return new DefineNode (name, arguments, this.ParseBody (), mode);
        }

        private INode   ParseKeywordDump ()
        {
            return new DumpNode (this.ParseExpression ());
        }

        private INode   ParseKeywordEcho ()
        {
            return new EchoNode (this.ParseExpression ());
        }

        private INode   ParseKeywordFor ()
        {
            INode           body;
            INode           empty;
            IExpression     from;
            NameExpression  key;
            NameExpression  value;

            key = this.ParseName ();

            if (this.lexer.Current.Type == Lexer.LexemType.COMMA)
            {
                this.lexer.Next ();

                value = this.ParseName ();
            }
            else
            {
                value = key;
                key = null;
            }

            this.ParseUnused (Lexer.LexemType.LITERAL, "in", "'in' keyword");

            from = this.ParseExpression ();
            body = this.ParseBody ();

            if (this.lexer.Current.Type == Lexer.LexemType.PIPE)
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

            while (fallback == null && this.lexer.Current.Type == Lexer.LexemType.PIPE)
            {
                this.lexer.Next ();

                switch (this.lexer.Current.Type == Lexer.LexemType.LITERAL ? this.lexer.Current.Data : string.Empty)
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

        private INode   ParseKeywordReturn ()
        {
            return new ReturnNode (this.ParseExpression ());
        }

        private INode   ParseKeywordSet ()
        {
            NameExpression  name = this.ParseName ();
            Scope.SetMode   mode;

            switch (this.lexer.Current.Type == Lexer.LexemType.LITERAL ? this.lexer.Current.Data : string.Empty)
            {
                case "as":
                    this.lexer.Next ();

                    mode = Scope.SetMode.LOCAL;

                    break;

                case "to":
                    this.lexer.Next ();

                    mode = Scope.SetMode.ANYWHERE;

                    break;

                default:
                    throw new UnexpectedException (this.lexer, "'as' or 'to' keyword");
            }

            return new SetNode (name, this.ParseExpression (), mode);
        }

        private INode   ParseKeywordWhile ()
        {
            IExpression test = this.ParseExpression ();

            return new WhileNode (test, this.ParseBody ());
        }

        private NameExpression  ParseName ()
        {
            NameExpression  name;

            if (this.lexer.Current.Type != Lexer.LexemType.LITERAL)
                throw new UnexpectedException (this.lexer, "variable name");

            name = new NameExpression (this.lexer.Current.Data);

            this.lexer.Next ();

            return name;
        }

        private INode   ParseRaw ()
        {
            List<INode> nodes = new List<INode> ();
            INode       node;

            while (true)
            {
                switch (this.lexer.Current.Type)
                {
                    case Lexer.LexemType.BRACE_END:
                    case Lexer.LexemType.PIPE:
                    case Lexer.LexemType.EOF:
                        this.lexer.Mode (Lexer.LexerMode.BLOCK);

                        return nodes.Count != 1 ? new CompositeNode (nodes) : nodes[0];

                    case Lexer.LexemType.BRACE_BEGIN:
                        this.lexer.Mode (Lexer.LexerMode.BLOCK);
                        this.lexer.Next ();

                        node = this.ParseBlock ();

                        if (node != null)
                            nodes.Add (node);

                        break;

                    case Lexer.LexemType.TEXT:
                        if (!string.IsNullOrEmpty (this.lexer.Current.Data))
                            nodes.Add (new TextNode (this.lexer.Current.Data));

                        this.lexer.Next ();

                        break;

                    default:
                        throw new UnexpectedException (this.lexer, "text or block begin ('{')");
                }
            }
        }

        private void    ParseUnused (Lexer.LexemType type, string value, string expected)
        {
            if (this.lexer.Current.Type != type || this.lexer.Current.Data != value)
                throw new UnexpectedException (this.lexer, expected);

            this.lexer.Next ();
        }

        #endregion

        #region Types

        private delegate INode  Keyword (Parser parser);

        #endregion
    }
}
