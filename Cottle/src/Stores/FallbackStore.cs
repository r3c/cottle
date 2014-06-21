using System;

namespace Cottle.Stores
{
	public sealed class FallbackStore : AbstractStore
	{
		#region Properties

		public IStore Constant
		{
			get
			{
				return this.constant;
			}
		}

		public IStore Mutable
		{
			get
			{
				return this.mutable;
			}
		}

		#endregion
		
		#region Attributes

		private readonly IStore	constant;

		private readonly IStore	mutable;

		#endregion
		
		#region Constructors

		public FallbackStore (IStore constant, IStore mutable)
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
		
		public override bool Leave ()
		{
			return this.mutable.Leave ();
		}
		
		public override void Set (Value symbol, Value value, StoreMode mode)
		{
			this.mutable.Set (symbol, value, mode);
		}

		public override bool TryGet (Value symbol, out Value value)
		{
			return this.mutable.TryGet (symbol, out value) || this.constant.TryGet (symbol, out value);
		}

		#endregion
	}
}
