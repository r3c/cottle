using System;
using System.Threading;

using Cottle.Commons;

namespace	Cottle.Scopes
{
	public class	DefaultScope : FallbackScope
	{
		#region Attributes
		
		private static IScope	constant = null;

		private static object	mutex = new object ();
		
		#endregion

		#region Constructors

		public	DefaultScope () :
			base (DefaultScope.GetConstant (), new SimpleScope ())
		{
		}

		#endregion

		#region Methods

		private static IScope	GetConstant ()
		{
			IScope	scope;

			if (DefaultScope.constant == null)
			{
				lock (DefaultScope.mutex)
				{
					if (DefaultScope.constant == null)
					{
						scope = new SimpleScope ();

						CommonFunctions.Assign (scope, ScopeMode.Closest);

						Interlocked.Exchange (ref DefaultScope.constant, scope);
					}
				}
			}

			return DefaultScope.constant;
		}

		#endregion
	}
}
