using System.Collections.Generic;
using System.IO;

using Cottle.Nodes.Generics;

namespace   Cottle.Nodes
{
    class   IfNode : Node
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

        public override void    Debug (TextWriter writer)
        {
            bool    first = true;

            foreach (Branch branch in this.branches)
            {
                writer.Write (string.Format (first ? "{{if {0}:" : "|elif {0}:", branch.Test));

                branch.Body.Debug (writer);

                first = false;
            }

            if (this.fallback != null)
            {
                writer.Write ("|else:");

                this.fallback.Debug (writer);
            }

            writer.Write ("}");
        }

        public override void    Print (Scope scope, TextWriter writer)
        {
            foreach (Branch branch in this.branches)
            {
                if (branch.Test.Evaluate (scope).AsBoolean)
                {
                    branch.Body.Print (scope, writer);

                    return;
                }
            }

            if (this.fallback != null)
                this.fallback.Print (scope, writer);
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
