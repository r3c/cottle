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

        private IExpression ParseExpression ()
        {
            List<IExpression>   arguments;
            IExpression         evaluable;
            List<VarExpression> fields;
            decimal             number;
            VarExpression       variable;

            switch (this.lexer.Type)
            {
                case Lexer.LexemType.LITERAL:
                    variable = this.ParseVariable ();

                    switch (this.lexer.Type)
                    {
                        case Lexer.LexemType.ARGUMENT_BEGIN:
                            this.lexer.Next ();

                            arguments = new List<IExpression> ();

                            while (this.lexer.Type != Lexer.LexemType.ARGUMENT_END)
                            {
                                arguments.Add (this.ParseExpression ());

                                if (this.lexer.Type == Lexer.LexemType.ARGUMENT_NEXT)
                                    this.lexer.Next ();
                            }

                            this.lexer.Next ();

                            evaluable = new FunctionExpression (variable, arguments);

                            break;

                        default:
                            fields = new List<VarExpression> ();
                            fields.Add (variable);

                            while (this.lexer.Type == Lexer.LexemType.FIELD)
                            {
                                this.lexer.Next ();

                                fields.Add (this.ParseVariable ());
                            }

                            evaluable = new AccessExpression (fields);

                            break;
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
            return new EchoNode (this.ParseExpression ());
        }

        private INode   ParseKeywordFor ()
        {
            INode           body;
            INode           empty;
            VarExpression   first;
            IExpression     from;
            VarExpression   key;
            VarExpression   value;

            first = this.ParseVariable ();

            if (this.lexer.Type == Lexer.LexemType.ARGUMENT_NEXT)
            {
                this.lexer.Next ();

                key = first;
                value = this.ParseVariable ();
            }
            else
            {
                key = null;
                value = first;
            }

            this.ParseUnused (Lexer.LexemType.LITERAL, "in", "'in' keyword");

            from = this.ParseExpression ();
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
            List<IfNode.Branch> branches = new List<IfNode.Branch> ();
            INode               fallback = null;
            IExpression         test;

            test = this.ParseExpression ();

            branches.Add (new IfNode.Branch (test, this.ParseBody ()));

            while (fallback == null && this.lexer.Type == Lexer.LexemType.BLOCK_NEXT)
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
            VarExpression   alias;
            IExpression     value;

            alias = this.ParseVariable ();

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

        private VarExpression   ParseVariable ()
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

        #endregion

        #region Types

        private delegate INode  Keyword (Parser parser);

        #endregion
    }
}
