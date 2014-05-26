using System;

namespace Cottle.Scopes
{
	public class FallbackScope : AbstractScope
	{
		#region Properties

		public IScope Constant
		{
			get
			{
				return this.constant;
			}
		}

		public IScope Mutable
		{
			get
			{
				return this.mutable;
			}
		}

		#endregion
		
		#region Attributes

		private readonly IScope	constant;

		private readonly IScope	mutable;

		#endregion
		
		#region Constructors

		public	FallbackScope (IScope constant, IScope mutable)
		{
			this.constant = constant;
			this.mutable = mutable;
		}

		#endregion

		#region Methods

		public override void Enter ()
		{
			this.mutable.Enter ();
		}
		
		public override bool Get (Value symbol, out Value value)
		{
			return this.mutable.Get (symbol, out value) || this.constant.Get (symbol, out value);
		}
		
		public override bool Leave ()
		{
			return this.mutable.Leave ();
		}
		
		public override void Set (Value symbol, Value value, ScopeMode mode)
		{
			this.mutable.Set (symbol, value, mode);
		}

		#endregion

		#region Obsoletes

		[Obsolete("Use 'Mutable' property")]
		public IScope Front
		{
			get
			{
				return this.mutable;
			}
		}

		#endregion
	}
}
