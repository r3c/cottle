using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Exceptions;
using Cottle.Expressions;
using Cottle.Lexers;
using Cottle.Nodes;

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
            {"set",     p => p.ParseKeywordSet ()}
        };

        #endregion

        #region Properties

        public Dictionary<string, Function> Functions
        {
            get
            {
                return this.functions;
            }
        }

        #endregion

        #region Attributes

        private Dictionary<string, Function>    functions = new Dictionary<string, Function> ();

        private Lexer                           lexer = new Lexer ();

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

        private VarExpression   ParseAssignable ()
        {
            VarExpression   assignable;

            switch (this.lexer.Type)
            {
                case Lexer.LexemType.LITERAL:
                case Lexer.LexemType.NUMBER:
                    assignable = new VarExpression (this.lexer.Value);

                    this.lexer.Next ();

                    return assignable;

                default:
                    throw new UnexpectedException (this.lexer, "variable name");
            }
        }

        private INode   ParseBlock ()
        {
            Keyword keyword;
            INode   node;

            if (this.lexer.Type != Lexer.LexemType.LITERAL)
                throw new UnexpectedException (this.lexer, "keyword");

            if (Parser.KEYWORDS.TryGetValue (this.lexer.Value, out keyword))
                this.lexer.Next ();
            else
                keyword = p => p.ParseKeywordEcho ();

            node = keyword (this);

            if (this.lexer.Type != Lexer.LexemType.BLOCK_END)
                throw new UnexpectedException (this.lexer, "end block character ('}')");

            this.lexer.Mode (Lexer.LexerMode.RAW);
            this.lexer.Next ();

            return node;
        }

        private INode   ParseBody ()
        {
            if (this.lexer.Type != Lexer.LexemType.BLOCK_BODY)
                throw new UnexpectedException (this.lexer, "body separator (':')");

            this.lexer.Mode (Lexer.LexerMode.RAW);
            this.lexer.Next ();

            return this.ParseRaw ();
        }

        private IExpression ParseEvaluable ()
        {
            List<IExpression>   arguments;
            IExpression         evaluable;
            List<VarExpression> fields;
            Function            function;
            decimal             number;

            switch (this.lexer.Type)
            {
                case Lexer.LexemType.LITERAL:
                    if (this.functions.TryGetValue (this.lexer.Value, out function))
                    {
                        this.lexer.Next ();

                        if (this.lexer.Type != Lexer.LexemType.ARGUMENT_BEGIN)
                            throw new UnexpectedException (this.lexer, "begin arguments character ('(')");

                        this.lexer.Next ();

                        arguments = new List<IExpression> ();

                        while (this.lexer.Type != Lexer.LexemType.ARGUMENT_END)
                        {
                            arguments.Add (this.ParseEvaluable ());

                            if (this.lexer.Type == Lexer.LexemType.ARGUMENT_NEXT)
                                this.lexer.Next ();
                        }

                        this.lexer.Next ();

                        evaluable = new FunctionExpression (function, arguments);
                    }
                    else
                    {
                        fields = new List<VarExpression> ();
                        fields.Add (this.ParseAssignable ());

                        while (this.lexer.Type == Lexer.LexemType.FIELD)
                        {
                            this.lexer.Next ();

                            fields.Add (this.ParseAssignable ());
                        }

                        evaluable = new AccessExpression (fields);
                    }

                    break;

                case Lexer.LexemType.NUMBER:
                    evaluable = new NumberExpression (decimal.TryParse (this.lexer.Value, out number) ? number : default (decimal));

                    this.lexer.Next ();

                    break;

                case Lexer.LexemType.STRING:
                    evaluable = new StringExpression (this.lexer.Value);

                    this.lexer.Next ();

                    break;

                default:
                    throw new UnexpectedException (this.lexer, "expression");
            }

            return evaluable;
        }

        private INode   ParseKeywordEcho ()
        {
            return new EchoNode (this.ParseEvaluable ());
        }

        private INode   ParseKeywordFor ()
        {
            INode           body;
            INode           empty;
            VarExpression   first;
            IExpression     from;
            VarExpression   key;
            VarExpression   value;

            first = this.ParseAssignable ();

            if (this.lexer.Type == Lexer.LexemType.ARGUMENT_NEXT)
            {
                this.lexer.Next ();

                key = first;
                value = this.ParseAssignable ();
            }
            else
            {
                key = null;
                value = first;
            }

            this.ParseUnused (Lexer.LexemType.LITERAL, "in", "'in' keyword");

            from = this.ParseEvaluable ();
            body = this.ParseBody ();

            if (this.lexer.Type == Lexer.LexemType.BLOCK_NEXT)
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
            INode       bodyElse;
            INode       bodyIf;
            IExpression test;

            test = this.ParseEvaluable ();
            bodyIf = this.ParseBody ();

            if (this.lexer.Type == Lexer.LexemType.BLOCK_NEXT)
            {
                this.lexer.Next ();

                this.ParseUnused (Lexer.LexemType.LITERAL, "else", "'else' keyword");

                bodyElse = this.ParseBody ();
            }
            else
                bodyElse = null;

            return new IfNode (test, bodyIf, bodyElse);
        }

        private INode   ParseKeywordSet ()
        {
            VarExpression   alias;
            IExpression     value;

            alias = this.ParseAssignable ();

            this.ParseUnused (Lexer.LexemType.LITERAL, "to", "'to' keyword");

            value = this.ParseEvaluable ();

            return new SetNode (alias, value, this.ParseBody ());
        }

        private INode   ParseRaw ()
        {
            List<INode> nodes = new List<INode> ();

            while (true)
            {
                switch (this.lexer.Type)
                {
                    case Lexer.LexemType.BLOCK_END:
                    case Lexer.LexemType.BLOCK_NEXT:
                    case Lexer.LexemType.EOF:
                        this.lexer.Mode (Lexer.LexerMode.BLOCK);

                        return nodes.Count != 1 ? new CompositeNode (nodes) : nodes[0];

                    case Lexer.LexemType.BLOCK_BEGIN:
                        this.lexer.Mode (Lexer.LexerMode.BLOCK);
                        this.lexer.Next ();

                        nodes.Add (this.ParseBlock ());

                        break;

                    case Lexer.LexemType.LITERAL:
                        if (!string.IsNullOrEmpty (this.lexer.Value))
                            nodes.Add (new RawNode (this.lexer.Value));

                        this.lexer.Next ();

                        break;

                    default:
                        throw new UnexpectedException (this.lexer, "raw text or begin block character ('{')");
                }
            }
        }

        private void    ParseUnused (Lexer.LexemType type, string literal, string expected)
        {
            if (this.lexer.Type != type || this.lexer.Value != literal)
                throw new UnexpectedException (this.lexer, expected);

            this.lexer.Next ();
        }

        #endregion

        #region Types

        private delegate INode  Keyword (Parser parser);

        #endregion
    }
}
