using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cottle.Parsers.Post;

namespace Cottle.Parsers
{
	class PostParser : IParser
	{
		#region Attributes

		private readonly IOptimizer[] optimizers;

		private readonly IParser parser;

		#endregion

		#region Constructors

		public PostParser (IParser parser, IEnumerable<IOptimizer> optimizers)
		{
			this.optimizers = optimizers.ToArray ();
			this.parser = parser;
		}

		#endregion

		#region Methods / Public

		public Command Parse (TextReader reader)
		{
			return this.Optimize (this.parser.Parse (reader));
		}

		#endregion

		#region Methods / Private

		private Command Optimize (Command command)
		{
			foreach (IOptimizer optimizer in this.optimizers)
				command = optimizer.Optimize (command);

			if (command.Body != null)
				command.Body = this.Optimize (command.Body);

			if (command.Next != null)
				command.Next = this.Optimize (command.Next);

			if (command.Operand != null)
				command.Operand = this.Optimize (command.Operand);

			return command;
		}

		private Expression Optimize (Expression expression)
		{
			foreach (IOptimizer optimizer in this.optimizers)
				expression = optimizer.Optimize (expression);

			if (expression.Arguments != null)
			{
				for (int i = 0; i < expression.Arguments.Length; ++i)
					expression.Arguments[i] = this.Optimize (expression.Arguments[i]);
			}

			if (expression.Elements != null)
			{
				for (int i = 0; i < expression.Elements.Length; ++i)
				{
					expression.Elements[i].Key = this.Optimize (expression.Elements[i].Key);
					expression.Elements[i].Value = this.Optimize (expression.Elements[i].Value);
				}
			}

			if (expression.Source != null)
				expression.Source = this.Optimize (expression.Source);

			if (expression.Subscript != null)
				expression.Subscript = this.Optimize (expression.Subscript);

			return expression;
		}

		#endregion
	}
}
