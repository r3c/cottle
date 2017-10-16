using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Cottle.Values
{
	public sealed class ReflectionValue : ResolveValue
	{
		#region Attributes / Instance

		private readonly BindingFlags binding;

		private readonly object source;

		#endregion

		#region Attributes / Static

		private static readonly Dictionary<Type, ValueConverter> converters = new Dictionary<Type, ValueConverter>
		{
			{typeof (bool),		(s) => (bool)s},
			{typeof (byte),		(s) => (byte)s},
			{typeof (char),		(s) => (char)s},
			{typeof (double),	(s) => (double)s},
			{typeof (decimal),	(s) => (decimal)s},
			{typeof (float),	(s) => (float)s},
			{typeof (int),		(s) => (int)s},
			{typeof (long),		(s) => (long)s},
			{typeof (sbyte),	(s) => (sbyte)s},
			{typeof (short),	(s) => (short)s},
			{typeof (string),	(s) => (string)s},
			{typeof (uint),		(s) => (uint)s},
			{typeof (ulong),	(s) => (long)(ulong)s},
			{typeof (ushort),	(s) => (ushort)s}
		};

		private static readonly Dictionary<Type, List<MemberReader>> readers = new Dictionary<Type, List<MemberReader>> ();

		#endregion

		#region Constructors

		public ReflectionValue (object source, BindingFlags binding)
		{
			if (source == null)
				throw new ArgumentNullException ("source");

			this.binding = binding;
			this.source = source;
		}

		public ReflectionValue (object source) :
			this (source, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
		}

		#endregion

		#region Methods

		protected override Value Resolve ()
		{
			ValueConverter converter;
			List<Value> elements;
			Dictionary<Value, Value> fields;
			List<MemberReader> reader;
			Type type;

			type = this.source.GetType ();

			// Use converter for known primitive types
			if (ReflectionValue.converters.TryGetValue (type, out converter))
				return converter (this.source);

			// Return undefined value for other primitive types
			if (type.IsPrimitive)
				return VoidValue.Instance;

			// Convert elements to array if source object is enumerable
			if (this.source is IEnumerable)
			{
				elements = new List<Value> ();

				foreach (object element in (IEnumerable)this.source)
					elements.Add (new ReflectionValue (element));

				return elements;
			}

			// Otherwise, browse object fields and properties
			fields = new Dictionary<Value, Value> ();

			lock (ReflectionValue.readers)
			{
				if (!ReflectionValue.readers.TryGetValue (type, out reader))
				{
					reader = new List<MemberReader> ();

					foreach (FieldInfo field in type.GetFields (this.binding))
						reader.Add (new MemberReader (field, this.binding));

					foreach (PropertyInfo property in type.GetProperties (this.binding))
						reader.Add (new MemberReader (property, this.binding));

					ReflectionValue.readers[type] = reader;
				}
			}

			foreach (MemberReader extractor in reader)
				fields.Add (extractor.Name, extractor.Extract (this.source));

			return fields;
		}

		#endregion

		#region Types

		private struct MemberReader
		{
			#region Properties

			public string Name
			{
				get
				{
					return this.name;
				}
			}

			#endregion

			#region Attributes

			private readonly BindingFlags binding;

			private readonly Func<object, object> extractor;

			private readonly string name;

			#endregion

			#region Constructors

			public MemberReader (FieldInfo field, BindingFlags binding)
			{
				this.binding = binding;
				this.extractor = field.GetValue;
				this.name = field.Name;
			}

			public MemberReader (PropertyInfo property, BindingFlags binding)
			{
				MethodInfo method;

				method = property.GetGetMethod (true);

				this.binding = binding;
				this.extractor = (s) => method.Invoke (s, null);
				this.name = property.Name;
			}

			#endregion

			#region Methods

			public Value Extract (object source)
			{
				object value;

				value = this.extractor (source);

				if (value != null)
					return new ReflectionValue (value, this.binding);

				return VoidValue.Instance;
			}

			#endregion
		}

		private delegate Value ValueConverter (object source);

		#endregion
	}
}
