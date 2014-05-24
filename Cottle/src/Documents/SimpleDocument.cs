using System;
using System.Collections.Generic;
using System.IO;
using Cottle.Documents.Simple;
using Cottle.Documents.Simple.Evaluators;
using Cottle.Documents.Simple.Nodes;
using Cottle.Parsers;
using Cottle.Settings;

namespace Cottle.Documents
{
	/// <summary>
	/// Simple document renders templates using an interpreter. If offers
	/// better garbage collection and easier debugging but average rendering
	/// performance.
	/// </summary>
	public sealed class SimpleDocument : AbstractDocument
	{
		#region Attributes

		private readonly INode		renderer;

		private readonly ISetting	setting;

		#endregion

		#region Constructors

		public SimpleDocument (TextReader reader, ISetting setting)
		{
			IParser	parser;
			Block	root;

			parser = new DefaultParser (setting.BlockBegin, setting.BlockContinue, setting.BlockEnd);
			root = parser.Parse (reader);

			this.setting = setting;
			this.renderer = this.CompileBlock (root);
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

		public override Value Render (IScope scope, TextWriter writer)
		{
			Value	result;

			this.renderer.Render (scope, writer, out result);

			return result;
		}

		public override void Source (TextWriter writer)
		{
			this.renderer.Source (this.setting, writer);
		}

		#endregion

		#region Methods / Private

		private INode CompileBlock (Block block)
		{
			KeyValuePair<IEvaluator, INode>[]	branches;
			List<INode>							nodes;

			switch (block.Type)
			{
				case BlockType.AssignFunction:
					return new AssignFunctionNode (block.Value, block.Arguments, this.CompileBlock (block.Body), block.Mode);

				case BlockType.AssignValue:
					return new AssignValueNode (block.Value, this.CompileExpression (block.Source), block.Mode);

				case BlockType.Composite:
					nodes = new List<INode> ();

					for (; block.Type == BlockType.Composite; block = block.Next)
						nodes.Add (this.CompileBlock (block.Body));

					nodes.Add (this.CompileBlock (block));

					return new CompositeNode (nodes);

				case BlockType.Dump:
					return new DumpNode (this.CompileExpression (block.Source));

				case BlockType.Echo:
					return new EchoNode (this.CompileExpression (block.Source));

				case BlockType.For:
					return new ForNode (this.CompileExpression (block.Source), block.Key, block.Value, this.CompileBlock (block.Body), block.Next != null ? this.CompileBlock (block.Next) : null);

				case BlockType.Literal:
					return new LiteralNode (this.setting.Trimmer (block.Text));

				case BlockType.Return:
					return new ReturnNode (this.CompileExpression (block.Source));

				case BlockType.Test:
					branches = new KeyValuePair<IEvaluator, INode>[block.Branches.Length];

					for (int i = 0; i < branches.Length; ++i)
						branches[i] = new KeyValuePair<IEvaluator, INode> (this.CompileExpression (block.Branches[i].Condition), this.CompileBlock (block.Branches[i].Body));

					return new TestNode (branches, block.Next != null ? this.CompileBlock (block.Next) : null);

				case BlockType.While:
					return new WhileNode (this.CompileExpression (block.Source), this.CompileBlock (block.Body));

				default:
					return new LiteralNode (string.Empty);
			}
		}

		private IEvaluator CompileExpression (Expression expression)
		{
			IEvaluator[]							arguments;
			KeyValuePair<IEvaluator, IEvaluator>[]	elements;
			InvokeEvaluator							invoke;
			IEvaluator								key;
			IEvaluator								value;

			switch (expression.Type)
			{
				case ExpressionType.Access:
					return new AccessEvaluator (this.CompileExpression (expression.Source), this.CompileExpression (expression.Subscript));

				case ExpressionType.Map:
					elements = new KeyValuePair<IEvaluator, IEvaluator>[expression.Elements.Length];

					for (int i = 0; i < elements.Length; ++i)
					{
						key = this.CompileExpression (expression.Elements[i].Key);
						value = this.CompileExpression (expression.Elements[i].Value);

						elements[i] = new KeyValuePair<IEvaluator, IEvaluator> (key, value);
					}

					return new MapEvaluator (elements);

				case ExpressionType.Invoke:
					arguments = new IEvaluator[expression.Arguments.Length];

					for (int i = 0; i < arguments.Length; ++i)
						arguments[i] = this.CompileExpression (expression.Arguments[i]);

					invoke = new InvokeEvaluator (this.CompileExpression (expression.Source), arguments);
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

		#endregion
	}
}
