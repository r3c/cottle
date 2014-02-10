using System;
using System.Collections.Generic;
using System.IO;

using Cottle.Values;

namespace Cottle.Functions
{
	public sealed class CallbackFunction : IFunction
	{
		#region Attributes

		private readonly CallbackDelegate	callback;

		private readonly int				max;

		private readonly int				min;

		#endregion

		#region Constructors

		public	CallbackFunction (CallbackDelegate callback, int min, int max)
		{
			this.callback = callback;
			this.max = max;
			this.min = min;
		}

		public	CallbackFunction (CallbackDelegate callback, int exact) :
			this (callback, exact, exact)
		{
		}

		public	CallbackFunction (CallbackDelegate callback) :
			this (callback, 0, -1)
		{
		}

		#endregion

		#region Methods

		public Value	Execute (IList<Value> values, IScope scope, TextWriter output)
		{
			if (this.callback == null || this.min > values.Count || (this.max >= 0 && this.max < values.Count))
				return VoidValue.Instance;

			return this.callback (values, scope, output);
		}

		#endregion

		#region Types

		public delegate Value	CallbackDelegate (IList<Value> values, IScope scope, TextWriter output);

		#endregion

		#region Obsoletes

		[Obsolete ("Callback should expect an IScope as its second parameter")]
		public	CallbackFunction (LegacyCallbackDelegate callback, int min, int max) :
			this ((IList<Value> values, IScope scope, TextWriter writer) => callback (values, new Scope (scope), writer), min, max)
		{
		}

		[Obsolete ("Callback should expect an IScope as its second parameter")]
		public	CallbackFunction (LegacyCallbackDelegate callback, int exact) :
			this (callback, exact, exact)
		{
		}

		[Obsolete ("Callback should expect an IScope as its second parameter")]
		public	CallbackFunction (LegacyCallbackDelegate callback) :
			this (callback, 0, -1)
		{
		}

		public delegate Value	LegacyCallbackDelegate (IList<Value> values, Scope scope, TextWriter output);

		#endregion
	}
}
