using System;
using System.Collections.Generic;
using Cottle.Builtins;
using Cottle.Values;

namespace Cottle.Stores
{
	public sealed class BuiltinStore : AbstractStore
	{
		#region Attributes / Instance

		private readonly FallbackStore store;

		#endregion

		#region Attributes / Static

		private static volatile IStore	constant = null;

		private static readonly object	mutex = new object ();
		
		#endregion

		#region Constructors

		public BuiltinStore ()
		{
			this.store = new FallbackStore (BuiltinStore.GetConstant (), new SimpleStore ());
		}

		#endregion

		#region Methods / Public

		public override void Enter ()
		{
			this.store.Enter ();
		}

		public override bool Leave ()
		{
			return this.store.Leave ();
		}

		public override void Set (Value symbol, Value value, StoreMode mode)
		{
			this.store.Set (symbol, value, mode);
		}

		public override bool TryGet (Value symbol, out Value value)
		{
			return this.store.TryGet (symbol, out value);
		}

		#endregion

		#region Methods / Private

		private static IStore GetConstant ()
		{
			IStore	store;

			if (BuiltinStore.constant == null)
			{
				lock (BuiltinStore.mutex)
				{
					if (BuiltinStore.constant == null)
					{
						store = new SimpleStore ();

						foreach (KeyValuePair<string, IFunction> instance in BuiltinFunctions.Instances)
							store[instance.Key] = new FunctionValue (instance.Value);

						BuiltinStore.constant = store;
					}
				}
			}

			return BuiltinStore.constant;
		}

		#endregion
	}
}
