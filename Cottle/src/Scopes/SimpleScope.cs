using System;
using System.Collections.Generic;
using System.Text;

using Cottle.Scopes.Abstracts;
using Cottle.Values;

namespace	Cottle.Scopes
{
	public class	SimpleScope : AbstractScope
	{
		#region Attributes

		private Stack<HashSet<Value>>			levels = new Stack<HashSet<Value>> ();

		private Dictionary<Value, Stack<Value>>	stacks = new Dictionary<Value, Stack<Value>> ();

		#endregion

		#region Constructors

		public	SimpleScope ()
		{
			this.levels.Push (new HashSet<Value> ());
		}

		#endregion

		#region Methods

		public override void	Enter ()
		{
			this.levels.Push (new HashSet<Value> ());
		}

		public override bool	Get (Value name, out Value value)
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

		public override bool	Leave ()
		{
			Stack<Value>	stack;

			if (this.levels.Count < 2)
				return false;

			foreach (Value name in this.levels.Pop ())
			{
				if (this.stacks.TryGetValue (name, out stack))
					stack.Pop ();
			}

			return true;
		}

		public override bool	Set (Value name, Value value, ScopeMode mode)
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
