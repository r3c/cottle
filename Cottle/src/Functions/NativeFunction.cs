using System;
using System.Collections.Generic;
using System.IO;

using Cottle.Values;

namespace Cottle.Functions
{
	public sealed class NativeFunction : IFunction
	{
		#region Attributes

		private readonly Func<IList<Value>, IScope, TextWriter, Value>	callback;

		private readonly int	max;

		private readonly int	min;

		#endregion

		#region Constructors

		public NativeFunction (Func<IList<Value>, IScope, TextWriter, Value> callback, int min, int max)
		{
			if (callback == null)
				throw new ArgumentNullException ("callback");

			this.callback = callback;
			this.max = max;
			this.min = min;
		}

		public NativeFunction (Func<IList<Value>, IScope, TextWriter, Value> callback, int exact) :
			this (callback, exact, exact)
		{
		}

		public NativeFunction (Func<IList<Value>, IScope, TextWriter, Value> callback) :
			this (callback, 0, -1)
		{
		}

		public NativeFunction (Func<IList<Value>, IScope, Value> callback, int min, int max) :
			this ((v, s, o) => callback (v, s), min, max) 
		{
		}

		public NativeFunction (Func<IList<Value>, IScope, Value> callback, int exact) :
			this ((v, s, o) => callback (v, s), exact) 
		{
		}

		public NativeFunction (Func<IList<Value>, IScope, Value> callback) :
			this ((v, s, o) => callback (v, s)) 
		{
		}

		public NativeFunction (Func<IList<Value>, Value> callback, int min, int max) :
			this ((v, s, o) => callback (v), min, max) 
		{
		}

		public NativeFunction (Func<IList<Value>, Value> callback, int exact) :
			this ((v, s, o) => callback (v), exact) 
		{
		}

		public NativeFunction (Func<IList<Value>, Value> callback) :
			this ((v, s, o) => callback (v)) 
		{
		}

		#endregion

		#region Methods

		public Value Execute (IList<Value> arguments, IScope scope, TextWriter output)
		{
			if (this.min > arguments.Count || (this.max >= 0 && this.max < arguments.Count))
				return VoidValue.Instance;

			return this.callback (arguments, scope, output);
		}

		#endregion
	}
}
