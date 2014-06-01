using System;
using System.Collections.Generic;

namespace Cottle.Parsers.Post.Optimizers
{
	/// <summary>
	/// Converts constant map expressions to constant expressions. 
	/// </summary>
	class ConstantMapOptimizer : AbstractOptimizer
	{
		#region Attributes

		private static readonly Predicate<ExpressionElement>	constants = (e) => e.Key.Type == ExpressionType.Constant && e.Value.Type == ExpressionType.Constant;

		#endregion

		#region Methods

		public override Expression Optimize (Expression expression)
		{
			KeyValuePair<Value, Value>[]	pairs;

			if (expression.Type == ExpressionType.Map && Array.TrueForAll (expression.Elements, ConstantMapOptimizer.constants))
			{
				pairs = new KeyValuePair<Value, Value>[expression.Elements.Length];

				for (int i = expression.Elements.Length; i-- > 0; )
					pairs[i] = new KeyValuePair<Value, Value> (expression.Elements[i].Key.Value, expression.Elements[i].Value.Value);

				return new Expression
				{
					Type	= ExpressionType.Constant,
					Value	= pairs
				};
			}

			return expression;
		}

		#endregion
	}
}
