using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Cottle.Documents.Dynamic
{
	class Allocator
	{
		#region Properties

		public ILGenerator			Generator
		{
			get
			{
				return this.generator;
			}
		}

		public LocalBuilder			LocalValue
		{
			get
			{
				if (this.localValue == null)
					this.localValue = this.generator.DeclareLocal (typeof (Value));

				return this.localValue;
			}
		}

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

		#region Attributes

		public readonly ILGenerator		generator;

		private LocalBuilder			localValue;

		private readonly List<string>	strings;

		private readonly List<Value>	values;

		#endregion

		#region Constructors

		public Allocator (ILGenerator generator)
		{
			this.generator = generator;
			this.localValue = null;
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
