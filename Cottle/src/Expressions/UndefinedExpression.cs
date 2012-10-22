using System.IO;

using Cottle.Expressions.Generics;
using Cottle.Values;

namespace   Cottle.Expressions
{
    sealed class    UndefinedExpression : Expression
    {
        #region Attributes

        public static readonly UndefinedExpression  Instance = new UndefinedExpression ();

        #endregion

        #region Methods

        public override Value   Evaluate (Scope scope, TextWriter output)
        {
            return UndefinedValue.Instance;
        }

        public override string  ToString ()
        {
            return UndefinedValue.Instance.ToString ();
        }

        #endregion
    }
}
