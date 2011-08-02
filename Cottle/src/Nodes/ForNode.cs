using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Expressions;
using Cottle.Nodes.Generics;
using Cottle.Values;

namespace   Cottle.Nodes
{
    class   ForNode : Node
    {
        #region Attributes

        private INode           body;

        private INode           empty;

        private IExpression     from;

        private VarExpression   key;

        private VarExpression   value;

        #endregion

        #region Constructors

        public  ForNode (IExpression from, VarExpression key, VarExpression value, INode body, INode empty)
        {
            this.body = body;
            this.empty = empty;
            this.from = from;
            this.key = key;
            this.value = value;
        }

        #endregion

        #region Methods

        public override void    Debug (DebugWriter writer)
        {
            if (this.key != null)
                writer.Write (string.Format ("{{for {0}, {1} in {2}:", this.key, this.value, this.from));
            else
                writer.Write (string.Format ("{{for {0} in {1}:", this.value, this.from));

            writer.Increase ();

            this.body.Debug (writer);

            writer.Decrease ();

            if (this.empty != null)
            {
                writer.Write ("|else:");
                writer.Increase ();

                this.empty.Debug (writer);

                writer.Decrease ();
            }

            writer.Write ("}");
        }

        public override void    Print (Scope scope, TextWriter writer)
        {
            IDictionary<string, IValue> children = this.from.Evaluate (scope).Children;

            if (children.Count > 0)
            {
                scope.Enter ();

                foreach (KeyValuePair<string, IValue> pair in children)
                {
                    if (this.key != null)
                        this.key.Set (scope, new StringValue (pair.Key));

                    if (this.value != null)
                        this.value.Set (scope, pair.Value);

                    this.body.Print (scope, writer);
                }

                scope.Leave ();
            }
            else if (this.empty != null)
                this.empty.Print (scope, writer);
        }

        #endregion
    }
}
