using System;
using System.IO;
using Cottle.Functions;
using Cottle.Stores;

namespace Cottle.Parsers.Post.Optimizers
{
	/// <summary>
	/// Evaluate constant invoke expressions using pure functions.
	/// </summary>
	class ConstantInvokeOptimizer : AbstractOptimizer
	{
		#region Attributes / Static

		public static readonly ConstantInvokeOptimizer Instance = new ConstantInvokeOptimizer ();

		#endregion

		#region Methods

		public override Expression Optimize (Expression expression)
		{
			NativeFunction function;

			if (expression.Type != ExpressionType.Invoke ||
			    !Array.TrueForAll (expression.Arguments, a => a.Type == ExpressionType.Constant) ||
			    expression.Source.Type != ExpressionType.Constant ||
			    expression.Source.Value.Type != ValueContent.Function)
				return expression;

			function = expression.Source.Value.AsFunction as NativeFunction;

			if (function == null || !function.Pure)
				return expression;

			return new Expression
			{
				Type	= ExpressionType.Constant,
				Value	= function.Execute (Array.ConvertAll (expression.Arguments, a => a.Value), new SimpleStore (), new StringWriter ()) 
			};
		}

		#endregion
	}
}
