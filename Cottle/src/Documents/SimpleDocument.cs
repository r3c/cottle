using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Cottle.Documents.Simple;
using Cottle.Documents.Simple.Evaluators;
using Cottle.Documents.Simple.Nodes;
using Cottle.Parsers;
using Cottle.Settings;

namespace Cottle.Documents
{
	public sealed class SimpleDocument : IDocument
	{
		#region Events

		public event DocumentError	Error;

		#endregion

		#region Attributes

		private readonly INode		node;

		private readonly ISetting	setting;

		#endregion

		#region Constructors

		public SimpleDocument (TextReader reader, ISetting setting)
		{
			IParser	parser;
			Block	root;

			parser = new DefaultParser (setting.BlockBegin, setting.BlockContinue, setting.BlockEnd);
			root = parser.Parse (reader);

			this.node = this.CompileNode (root);
			this.setting = setting;
		}

		public SimpleDocument (TextReader reader) :
			this (reader, DefaultSetting.Instance)
		{
		}

		public SimpleDocument (string template, ISetting setting) :
			this (new StringReader (template), setting)
		{
		}

		public SimpleDocument (string template) :
			this (new StringReader (template), DefaultSetting.Instance)
		{
		}

		#endregion

		#region Methods / Public

		public Value Render (IScope scope, TextWriter writer)
		{
			Value	result;

			this.node.Render (scope, writer, out result);

			return result;
		}

		public string Render (IScope scope)
		{
			StringWriter	writer;

			writer = new StringWriter (CultureInfo.InvariantCulture);

			this.Render (scope, writer);

			return writer.ToString ();
		}

		public void	Source (TextWriter writer)
		{
			this.node.Source (this.setting, writer);
		}

		public string Source ()
		{
			StringWriter	writer;

			writer = new StringWriter (CultureInfo.InvariantCulture);

			this.Source (writer);

			return writer.ToString ();
		}

		#endregion

		#region Methods / Private

		private IEvaluator CompileEvaluator (Expression expression)
		{
			IEvaluator[]							arguments;
			KeyValuePair<IEvaluator, IEvaluator>[]	elements;
			InvokeEvaluator							invoke;
			IEvaluator								key;
			IEvaluator								value;

			switch (expression.Type)
			{
				case ExpressionType.Access:
					return new AccessEvaluator (this.CompileEvaluator (expression.Source), this.CompileEvaluator (expression.Subscript));

				case ExpressionType.Map:
					elements = new KeyValuePair<IEvaluator, IEvaluator>[expression.Elements.Length];

					for (int i = 0; i < elements.Length; ++i)
					{
						key = this.CompileEvaluator (expression.Elements[i].Key);
						value = this.CompileEvaluator (expression.Elements[i].Value);

						elements[i] = new KeyValuePair<IEvaluator, IEvaluator> (key, value);
					}

					return new MapEvaluator (elements);

				case ExpressionType.Invoke:
					arguments = new IEvaluator[expression.Arguments.Length];

					for (int i = 0; i < arguments.Length; ++i)
						arguments[i] = this.CompileEvaluator (expression.Arguments[i]);

					invoke = new InvokeEvaluator (this.CompileEvaluator (expression.Source), arguments);
					invoke.Error += this.OnError;

					return invoke;

				case ExpressionType.Name:
					return new NameEvaluator (expression.String);

				case ExpressionType.Number:
					return new ConstantEvaluator (expression.Number);

				case ExpressionType.String:
					return new ConstantEvaluator (expression.String);

				default:
					return VoidEvaluator.Instance;
			}
		}

		private INode CompileNode (Block block)
		{
			KeyValuePair<IEvaluator, INode>[]	branches;
			List<INode>							nodes;

			switch (block.Type)
			{
				case BlockType.AssignFunction:
					return new AssignFunctionNode (block.Value, block.Arguments, this.CompileNode (block.Body), block.Mode);

				case BlockType.AssignValue:
					return new AssignValueNode (block.Value, this.CompileEvaluator (block.Source), block.Mode);

				case BlockType.Composite:
					nodes = new List<INode> ();

					for (; block.Type == BlockType.Composite; block = block.Next)
						nodes.Add (this.CompileNode (block.Body));

					nodes.Add (this.CompileNode (block));

					return new CompositeNode (nodes);

				case BlockType.Dump:
					return new DumpNode (this.CompileEvaluator (block.Source));

				case BlockType.Echo:
					return new EchoNode (this.CompileEvaluator (block.Source));

				case BlockType.For:
					return new ForNode (this.CompileEvaluator (block.Source), block.Key, block.Value, this.CompileNode (block.Body), block.Next != null ? this.CompileNode (block.Next) : null);

				case BlockType.Literal:
					return new LiteralNode (this.setting.Trimmer (block.Text));

				case BlockType.Return:
					return new ReturnNode (this.CompileEvaluator (block.Source));

				case BlockType.Test:
					branches = new KeyValuePair<IEvaluator, INode>[block.Branches.Length];

					for (int i = 0; i < branches.Length; ++i)
						branches[i] = new KeyValuePair<IEvaluator, INode> (this.CompileEvaluator (block.Branches[i].Condition), this.CompileNode (block.Branches[i].Body));

					return new TestNode (branches, block.Next != null ? this.CompileNode (block.Next) : null);

				case BlockType.While:
					return new WhileNode (this.CompileEvaluator (block.Source), this.CompileNode (block.Body));

				default:
					return new LiteralNode (string.Empty);
			}
		}

		private void OnError (string source, string message, Exception exception)
		{
			DocumentError	error;

			error = this.Error;

			if (error != null)
				error (source, message, exception);
		}

		#endregion
	}
}
