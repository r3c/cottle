using System;

namespace Cottle.Values
{
	public sealed class LazyValue : ResolveValue
	{
		#region Attributes

		private readonly Func<Value> resolver;

		#endregion

		#region Constructors

		public LazyValue (Func<Value> resolver)
		{
			this.resolver = resolver;
		}

		#endregion

		#region Methods

		protected override Value Resolve ()
		{
			return this.resolver ();
		}

		#endregion
	}
}
