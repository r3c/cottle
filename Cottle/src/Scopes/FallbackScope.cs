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
		
		public override bool Get (Value name, out Value value)
		{
			return this.mutable.Get (name, out value) || this.constant.Get (name, out value);
		}
		
		public override bool Leave ()
		{
			return this.mutable.Leave ();
		}
		
		public override bool Set (Value name, Value value, ScopeMode mode)
		{
			return this.mutable.Set (name, value, mode);
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
