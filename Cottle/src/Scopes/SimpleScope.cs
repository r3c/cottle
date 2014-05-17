using System;
using System.Collections.Generic;

namespace Cottle.Scopes
{
	public sealed class SimpleScope : AbstractScope
	{
		#region Attributes

		private readonly Stack<HashSet<Value>>				levels;

		private readonly Dictionary<Value, Stack<Value>>	stacks;

		#endregion

		#region Constructors

		public SimpleScope ()
		{
			this.levels = new Stack<HashSet<Value>> ();
			this.levels.Push (new HashSet<Value> ());
			this.stacks = new Dictionary<Value, Stack<Value>> ();
		}

		#endregion

		#region Methods

		public override void Enter ()
		{
			this.levels.Push (new HashSet<Value> ());
		}

		public override bool Get (Value name, out Value value)
		{
			Stack<Value>	stack;

			if (this.stacks.TryGetValue (name, out stack) && stack.Count > 0)
			{
				value = stack.Peek ();

				return true;
			}

			value = null;

			return false;
		}

		public override bool Leave ()
		{
			Stack<Value>	stack;

			if (this.levels.Count < 2)
				return false;

			foreach (Value name in this.levels.Pop ())
			{
				if (this.stacks.TryGetValue (name, out stack))
				{
					if (stack.Count < 2)
						this.stacks.Remove (name);
					else
						stack.Pop ();						
				}
			}

			return true;
		}

		public override bool Set (Value name, Value value, ScopeMode mode)
		{
			HashSet<Value>	level;
			Stack<Value>	stack;

			if (!this.stacks.TryGetValue (name, out stack))
			{
				stack = new Stack<Value> ();

				this.stacks[name] = stack;
			}

			level = this.levels.Peek ();

			switch (mode)
			{
				case ScopeMode.Closest:
					if (stack.Count > 0)
						stack.Pop ();

					break;

				case ScopeMode.Local:
					if (!level.Add (name))
						stack.Pop ();

					break;
			}

			stack.Push (value);

			return true;
		}

		#endregion
	}
}
