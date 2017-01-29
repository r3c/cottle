using System;
using System.Collections.Generic;

namespace Cottle.Stores
{
	public sealed class SimpleStore : AbstractStore
	{
		#region Attributes

		private readonly Stack<HashSet<Value>> levels;

		private readonly Dictionary<Value, Stack<Value>> stacks;

		#endregion

		#region Constructors

		public SimpleStore ()
		{
			this.levels = new Stack<HashSet<Value>> ();
			this.stacks = new Dictionary<Value, Stack<Value>> ();
		}

		#endregion

		#region Methods

		public override void Enter ()
		{
			this.levels.Push (new HashSet<Value> ());
		}

		public override bool Leave ()
		{
			Stack<Value> stack;

			if (this.levels.Count < 1)
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

		public override void Set (Value symbol, Value value, StoreMode mode)
		{
			Stack<Value> stack;

			if (!this.stacks.TryGetValue (symbol, out stack))
			{
				stack = new Stack<Value> ();

				this.stacks[symbol] = stack;
			}

			switch (mode)
			{
				case StoreMode.Global:
					if (stack.Count > 0)
						stack.Pop ();

					break;

				case StoreMode.Local:
					if (this.levels.Count > 0 && !this.levels.Peek ().Add (symbol))
						stack.Pop ();

					break;
			}

			stack.Push (value);
		}

		public override bool TryGet (Value symbol, out Value value)
		{
			Stack<Value> stack;

			if (this.stacks.TryGetValue (symbol, out stack) && stack.Count > 0)
			{
				value = stack.Peek ();

				return true;
			}

			value = null;

			return false;
		}

		#endregion
	}
}
