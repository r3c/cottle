using System.Collections.Generic;
using System.IO;

using Cottle.Nodes.Generics;

namespace   Cottle.Nodes
{
    sealed class    IfNode : Node
    {
        #region Attributes

        private IEnumerable<Branch> branches;

        private INode               fallback;

        #endregion

        #region Constructors

        public  IfNode (IEnumerable<Branch> branches, INode fallback)
        {
            this.branches = branches;
            this.fallback = fallback;
        }

        #endregion

        #region Methods

        public override IValue  Apply (Scope scope, TextWriter output)
        {
            foreach (Branch branch in this.branches)
            {
                if (branch.Test.Evaluate (scope, output).AsBoolean)
                    return branch.Body.Apply (scope, output);
            }

            if (this.fallback != null)
                return this.fallback.Apply (scope, output);

            return null;
        }

        public override void    Debug (TextWriter output)
        {
            bool    first = true;

            foreach (Branch branch in this.branches)
            {
                output.Write (first ? "{if " : "|elif");
                output.Write (branch.Test);
                output.Write (": ");

                branch.Body.Debug (output);

                first = false;
            }

            if (this.fallback != null)
            {
                output.Write ("|else:");

                this.fallback.Debug (output);
            }

            output.Write ('}');
        }

        #endregion

        #region Types

        public class    Branch
        {
            #region Properties

            public INode        Body
            {
                get
                {
                    return this.body;
                }
            }

            public IExpression  Test
            {
                get
                {
                    return this.test;
                }
            }

            #endregion

            #region Attributes

            private INode       body;

            private IExpression test;

            #endregion

            #region Constructors

            public  Branch (IExpression test, INode body)
            {
                this.body = body;
                this.test = test;
            }

            #endregion
        }

        #endregion
    }
}
