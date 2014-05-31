using System;
using System.Collections.Generic;
using Cottle.Builtins;
using Cottle.Values;

namespace Cottle.Scopes
{
	public sealed class DefaultScope : FallbackScope
	{
		#region Attributes

		private static volatile IScope	constant = null;

		private static readonly object	mutex = new object ();
		
		#endregion

		#region Constructors

		public	DefaultScope () :
			base (DefaultScope.GetConstant (), new SimpleScope ())
		{
		}

		#endregion

		#region Methods

		private static IScope GetConstant ()
		{
			IScope	scope;

			if (DefaultScope.constant == null)
			{
				lock (DefaultScope.mutex)
				{
					if (DefaultScope.constant == null)
					{
						scope = new SimpleScope ();

						foreach (KeyValuePair<string, IFunction> instance in BuiltinFunctions.Instances)
							scope[instance.Key] = new FunctionValue (instance.Value);

						DefaultScope.constant = scope;
					}
				}
			}

			return DefaultScope.constant;
		}

		#endregion
	}
}
