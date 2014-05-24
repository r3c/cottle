using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Cottle.Documents.Dynamic
{
	class Allocator
	{
		#region Properties

		public IEnumerable<string>	Strings
		{
			get
			{
				return this.strings;
			}
		}

		public IEnumerable<Value>	Values
		{
			get
			{
				return this.values;
			}
		}

		#endregion

		#region Attributes / Public

		public readonly LocalBuilder	LocalValue;

		#endregion

		#region Attributes / Private

		private readonly List<string>	strings;

		private readonly List<Value>	values;

		#endregion

		#region Constructors

		public Allocator (LocalBuilder localValue)
		{
			this.LocalValue = localValue;
			this.strings = new List<string> ();
			this.values = new List<Value> ();
		}

		#endregion

		#region Methods

		public int Allocate (string value)
		{
			this.strings.Add (value);

			return this.strings.Count - 1; 
		}

		public int Allocate (Value value)
		{
			this.values.Add (value);

			return this.values.Count - 1;
		}

		#endregion
	}
}
