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

		public LocalBuilder			LocalFunction
		{
			get
			{
				if (this.localFunction == null)
					this.localFunction = this.generator.DeclareLocal (typeof (IFunction));

				return this.localFunction;
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

		public Label				Terminate
		{
			get
			{
				return this.terminate;
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

		private readonly ILGenerator	generator;

		private LocalBuilder			localFunction;

		private LocalBuilder			localValue;

		private readonly Label			terminate;

		private readonly List<string>	strings;

		private readonly List<Value>	values;

		#endregion

		#region Constructors

		public Allocator (ILGenerator generator)
		{
			this.generator = generator;
			this.localFunction = null;
			this.localValue = null;
			this.terminate = generator.DefineLabel ();
			this.strings = new List<string> ();
			this.values = new List<Value> ();
		}

		#endregion

		#region Methods

		public int Allocate (string value)
		{
			int	index;

			// FIXME: slow
			index = this.strings.IndexOf (value);

			if (index < 0)
			{
				index = this.strings.Count;

				this.strings.Add (value);
			}

			return index;
		}

		public int Allocate (Value value)
		{
			int	index;

			// FIXME: slow
			index = this.values.IndexOf (value);

			if (index < 0)
			{
				index = this.values.Count;

				this.values.Add (value);
			}

			return index;
		}

		#endregion
	}
}
