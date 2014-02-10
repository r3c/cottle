using System;

using Cottle.Scopes.Abstracts;

namespace Cottle.Scopes
{
	public class FallbackScope : AbstractScope
	{
		#region Properties

		public IScope	Constant
		{
			get
			{
				return this.constant;
			}
		}

		public IScope	Front
		{
			get
			{
				return this.front;
			}
		}

		#endregion
		
		#region Attributes

		private readonly IScope	constant;

		private readonly IScope	front;

		#endregion
		
		#region Constructors

		public	FallbackScope (IScope constant, IScope front)
		{
			this.constant = constant;
			this.front = front;
		}

		#endregion

		#region Methods

		public override void	Enter ()
		{
			this.front.Enter ();
		}
		
		public override bool	Get (Value name, out Value value)
		{
			return this.front.Get (name, out value) || this.constant.Get (name, out value);
		}
		
		public override bool	Leave ()
		{
			return this.front.Leave ();
		}
		
		public override bool	Set (Value name, Value value, ScopeMode mode)
		{
			return this.front.Set (name, value, mode);
		}

		#endregion
	}
}
