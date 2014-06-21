using System;
using System.Collections.Generic;
using Cottle.Builtins;
using Cottle.Values;

namespace Cottle.Scopes
{
	[Obsolete("Use Cottle.Stores.BuiltinStore")]
	public sealed class BuiltinScope : FallbackScope
	{
		#region Attributes

		private static volatile IScope	constant = null;

		private static readonly object	mutex = new object ();
		
		#endregion

		#region Constructors

		public BuiltinScope () :
			base (BuiltinScope.GetConstant (), new SimpleScope ())
		{
		}

		#endregion

		#region Methods

		private static IScope GetConstant ()
		{
			IScope	scope;

			if (BuiltinScope.constant == null)
			{
				lock (BuiltinScope.mutex)
				{
					if (BuiltinScope.constant == null)
					{
						scope = new SimpleScope ();

						foreach (KeyValuePair<string, IFunction> instance in BuiltinFunctions.Instances)
							scope[instance.Key] = new FunctionValue (instance.Value);

						BuiltinScope.constant = scope;
					}
				}
			}

			return BuiltinScope.constant;
		}

		#endregion
	}
}
