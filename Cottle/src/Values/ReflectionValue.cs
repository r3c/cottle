using System;
using System.Collections.Generic;
using System.Reflection;

using Cottle.Values.Generics;
using System.Collections;

namespace   Cottle.Values
{
    public sealed class ReflectionValue : ResolveValue
    {
        #region Attributes / Instance

        private object  source;

        #endregion

        #region Attributes / Static

        private static readonly Dictionary<Type, ValueConverter>        converters = new Dictionary<Type, ValueConverter>
        {
            {typeof (bool),     (s) => (bool)s},
            {typeof (byte),     (s) => (byte)s},
            {typeof (char),     (s) => (char)s},
            {typeof (double),   (s) => (double)s},
            {typeof (float),    (s) => (float)s},
            {typeof (int),      (s) => (int)s},
            {typeof (long),     (s) => (long)s},
            {typeof (short),    (s) => (short)s},
            {typeof (string),   (s) => (string)s}
        };

        private static readonly Dictionary<Type, List<MemberReader>>    readers = new Dictionary<Type, List<MemberReader>> ();

        #endregion

        #region Constructors

        public  ReflectionValue (object source)
        {
            if (source == null)
                throw new ArgumentNullException ("source");

            this.source = source;
        }

        #endregion

        #region Methods / Public

        public override string  ToString ()
        {
            return "<reflexion>";
        }

        #endregion

        #region Methods / Protected

        protected override Value    Resolve()
        {
            ValueConverter              converter;
            List<Value>                 elements;
            Dictionary<Value, Value>    fields;
            List<MemberReader>          reader;
            Type                        type;

            type = this.source.GetType ();

            // Use converter for known primitive types
            if (ReflectionValue.converters.TryGetValue (type, out converter))
                return converter (this.source);

            // Return undefined value for other primitive types
            else if (type.IsPrimitive)
                return UndefinedValue.Instance;

            // Convert elements to array if source object is enumerable
            else if (this.source is IEnumerable)
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

                    foreach (FieldInfo field in type.GetFields (BindingFlags.Instance | BindingFlags.NonPublic))
                        reader.Add (new MemberReader (field));

                    foreach (PropertyInfo property in type.GetProperties (BindingFlags.Instance | BindingFlags.NonPublic))
                        reader.Add (new MemberReader (property));

                    ReflectionValue.readers[type] = reader;
                }
            }

            foreach (MemberReader extractor in reader)
                fields.Add (extractor.Name, extractor.Extract (this.source));

            return fields;
        }

        #endregion

        #region Types

        private struct  MemberReader
	    {
            #region Properties

            public string   Name
            {
                get
                {
                    return this.name;
                }
            }

            #endregion

            #region Attributes

            private Func<object, object>    extractor;

		    private string                  name;

            #endregion

            #region Constructors

            public  MemberReader (FieldInfo field)
            {
                this.extractor = (s) => field.GetValue (s);
                this.name = field.Name;
            }

            public  MemberReader (PropertyInfo property)
            {
                MethodInfo  method;

                method = property.GetGetMethod (true);

                this.extractor = (s) => method.Invoke (s, null);
                this.name = property.Name;
            }

            #endregion

            #region Methods

            public Value    Extract (object source)
            {
                object  value;

                value = this.extractor (source);

                if (value != null)
                    return new ReflectionValue (value);

                return UndefinedValue.Instance;
            }

            #endregion
	    }

        private delegate Value  ValueConverter (object source);

        #endregion
    }
}
