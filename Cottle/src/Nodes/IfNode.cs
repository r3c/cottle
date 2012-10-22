using System.Collections.Generic;
using System.IO;

using Cottle.Values;

namespace   Cottle.Nodes
{
    sealed class    IfNode : INode
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

        public bool Apply (Scope scope, TextWriter output, out Value result)
        {
            bool    halt;

            foreach (Branch branch in this.branches)
            {
                if (branch.Test.Evaluate (scope, output).AsBoolean)
                {
                    scope.Enter ();

                    halt = branch.Body.Apply (scope, output, out result);

                    scope.Leave ();

                    return halt;
                }
            }

            if (this.fallback != null)
            {
                scope.Enter ();

                halt = this.fallback.Apply (scope, output, out result);

                scope.Leave ();

                return halt;
            }

            result = UndefinedValue.Instance;

            return false;
        }

        public void Print (LexerConfig config, TextWriter output)
        {
            bool    first;
            
            first = true;

            foreach (Branch branch in this.branches)
            {
            	if (first)
            	{
            		output.Write (config.BlockBegin);
            		output.Write ("if ");

            		first = false;
            	}
            	else
            	{
            		output.Write (config.BlockContinue);
            		output.Write ("elif ");
            	}

                output.Write (branch.Test);
                output.Write (":");

                branch.Body.Print (config, output);
            }

            if (this.fallback != null)
            {
            	output.Write (config.BlockContinue);
                output.Write ("else:");

                this.fallback.Print (config, output);
            }

            output.Write (config.BlockEnd);
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
