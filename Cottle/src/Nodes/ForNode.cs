using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Expressions;
using Cottle.Nodes.Generics;
using Cottle.Values;

namespace   Cottle.Nodes
{
    sealed class    ForNode : Node
    {
        #region Attributes

        private INode           body;

        private INode           empty;

        private IExpression     from;

        private NameExpression   key;

        private NameExpression   value;

        #endregion

        #region Constructors

        public  ForNode (IExpression from, NameExpression key, NameExpression value, INode body, INode empty)
        {
            this.body = body;
            this.empty = empty;
            this.from = from;
            this.key = key;
            this.value = value;
        }

        #endregion

        #region Methods

        public override IValue  Apply (Scope scope, TextWriter output)
        {
            ICollection<KeyValuePair<IValue, IValue>>   collection = this.from.Evaluate (scope, output).Children;
            IValue                                      result;

            if (collection.Count > 0)
            {
                foreach (KeyValuePair<IValue, IValue> pair in collection)
                {
                    scope.Enter ();

                    if (this.key != null)
                        this.key.Set (scope, pair.Key, Scope.SetMode.LOCAL);

                    if (this.value != null)
                        this.value.Set (scope, pair.Value, Scope.SetMode.LOCAL);

                    result = this.body.Apply (scope, output);

                    scope.Leave ();

                    if (result != null)
                        return result;
                }
            }
            else if (this.empty != null)
                return this.empty.Apply (scope, output);

            return null;
        }

        public override void    Debug (TextWriter output)
        {
            output.Write ("{for ");

            if (this.key != null)
            {
                output.Write (this.key);
                output.Write (", ");
            }

            output.Write (this.value);
            output.Write (" in ");
            output.Write (this.from);
            output.Write (": ");

            this.body.Debug (output);

            if (this.empty != null)
            {
                output.Write ("|empty:");

                this.empty.Debug (output);
            }

            output.Write ('}');
        }

        #endregion
    }
}
