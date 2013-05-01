using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Expressions;
using Cottle.Values;

namespace	Cottle.Functions
{
	public class	CallbackFunction : IFunction
	{
		#region Attributes

		private CallbackDelegate	callback;

		private int					max;

		private int					min;

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
				return UndefinedValue.Instance;

			return this.callback (values, scope, output);
		}

		#endregion

		#region Types

		public delegate Value	CallbackDelegate (IList<Value> values, IScope scope, TextWriter output);

		#endregion
	}
}
