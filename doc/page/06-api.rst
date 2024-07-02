.. default-domain:: csharp
.. namespace:: Cottle

=============
API reference
=============

Public API definition
=====================

This page contains information about types that are part of Cottle's public API.

.. warning::

    Types not listed in this page should not be considered as part of the public API, and are not taken into consideration when changing version number (see section :ref:`versioning_convention`).

.. warning::

    You should avoid relying on method behaviors not documented in this page as they could change from one version to another.



Compiled documents
==================

.. namespace:: Cottle
.. class:: IDocument

    A document in Cottle is a compiled template, which means a template converted to an optimized in-memory representation.

    .. method:: Value Render(IContext context, System.IO.TextWriter writer)

        Render document and write output to given :type:`System.IO.TextWriter` instance. Return value is the value passed to top-level ``return`` command if any, or an undefined value otherwise.

    .. method:: string Render(IContext context)

        Render document and return outout as a :type:`System.String` instance.


.. namespace:: Cottle
.. class:: Document

    Methods from this static class must be used to create instances of :type:`DocumentResult`.

    .. method:: DocumentResult CreateDefault(System.IO.TextReader template, DocumentConfiguration configuration = default)

        Create a new default :type:`IDocument` instance suitable for most use cases. Template is read from any non-seekable :type:`System.IO.TextWriter` instance.

    .. method:: DocumentResult CreateDefault(string template, DocumentConfiguration configuration = default)

        Create a new default :type:`IDocument` instance similar to previous method. Template is read from given :type:`System.String` instance.

    .. method:: DocumentResult CreateNative(System.IO.TextReader template, DocumentConfiguration configuration = default)

        Create a new native :type:`IDocument` instance for better rendering performance but higher compilation cost. Template is read from any non-seekable :type:`System.IO.TextWriter` instance. See section :ref:`native_document` for details about native documents.

    .. method:: DocumentResult CreateNative(string template, DocumentConfiguration configuration = default)

        Create a new native :type:`IDocument` instance similar to previous method. Template is read from given :type:`System.String` instance.


.. namespace:: Cottle.Documents
.. class:: DynamicDocument

    .. inherits:: Cottle.IDocument

    Deprecated class, use :meth:`Cottle.Document.CreateNative` to create native documents.


.. namespace:: Cottle.Documents
.. class:: SimpleDocument

    .. inherits:: Cottle.IDocument

    Deprecated class, use :meth:`Cottle.Document.CreateDefault` to create documents.


.. namespace:: Cottle
.. class:: DocumentConfiguration

    Document configuration options, can be passed as an optional argument when creating a new document.

    .. property:: string BlockBegin { get; set; }

        Delimiter for *block begin*, see section :ref:`delimiter_customization` for details.

    .. property:: string BlockContinue { get; set; }

        Delimiter for *block continue*, see section :ref:`delimiter_customization` for details.

    .. property:: string BlockEnd { get; set; }

        Delimiter for *block end*, see section :ref:`delimiter_customization` for details.

    .. property:: System.Nullable<char> Escape { get; set; }

        Delimiter for *escape*, see section :ref:`delimiter_customization` for details. Default escape character is **\\** when this property is null.

    .. property:: int? NbCycleMax { get; set; }

        When set to a non-null value, a :type:`NbCycleExceededException` exception is thrown if template rendering requires more cycles than allowed, see :ref:`render_nb_cycle_max` for details.

    .. property:: bool NoOptimize { get; set; }

        Disable code optimizations after compiling a document, see :ref:`optimizer_deactivation` for details.

    .. property:: System.Func<string,string> Trimmer { get; set; }

        Function used to trim unwanted character out of plain text when parsing a document, see section :ref:`plain_text_trimming` for details.


.. namespace:: Cottle
.. class:: DocumentResult

    This structure holds result of a template compilation, which can either be successful and provide compiled :type:`IDocument` instance or failed and provide compilation error details as a list of :type:`DocumentReport` elements:

    .. property:: IDocument Document { get; }

        Instance of compiled document, only if compilation was successful (see :prop:`DocumentResult.Success`).

    .. property:: System.Collections.Generic.IReadOnlyList<DocumentReport> Reports { get; }

        List of anomalies detected during compilation, as a read-only list of :type:`DocumentReport` items.

    .. property:: bool Success { get; }

        Indicate whether compilation was successful or not.

    .. property:: IDocument DocumentOrThrow { get; }

        Helper to return compiled document when compilation was successful or throw a :type:`Exceptions.ParseException` exception with details about first compilation error otherwise.


.. namespace:: Cottle
.. class:: DocumentReport

    Anomaly report on compiled template, with references to related code location.

    .. property:: int Length { get; }

        Length of the last lexem recognized when encountering an anomaly.

    .. property:: string Message { get; }

        Human-readable description of the anomaly. This value is meant for being displayed in a user interface but not processed, as its contents is not predictable.

    .. property:: int Offset { get; }

        Offset of the last lexem recognized when encountering an anomaly.

    .. property:: DocumentSeverity Severity { get; }

        Report severity level.


.. namespace:: Cottle
.. enum:: DocumentSeverity

   Report severity level.

   .. value:: Error

        Template issue that prevents document from being constructed.

   .. value:: Warning

        Template issue that doesn't prevent document from being constructed nor rendered, but may impact rendered result or performance and require your attention.

   .. value:: Notice

        Template issue with no visible impact, mostly used for code suggestions or deprecation messages.



Rendering contexts
==================

.. namespace:: Cottle
.. class:: IContext

    This interface is used to pass variables to a document when rendering it.

    .. indexer:: Value this[Value symbol] { get; }

        Get variable by its symbol (usually its name), or an undefined value if no value was defined with this name.


.. namespace:: Cottle
.. class:: Context

    Methods from this static class must be used to create instances of :type:`IContext`.

    .. method:: IContext CreateBuiltin(IContext custom)

        Create a rendering context by combining a given existing context with all Cottle built-in functions (see section :ref:`builtin`). Variables from the input context always have priority over built-in functions in case of collision.

    .. method:: IContext CreateBuiltin(System.Collections.Generic.IReadOnlyDictionary<Value,Value> symbols)

        Create a rendering context by combining variables from given dictionary with all Cottle built-in functions. This method is similar to previous one and only exists as a convenience helper.

    .. method:: IContext CreateCascade(IContext primary, IContext fallback)

        Create a rendering context by combining two existing contexts that will be searched in order when querying a variable. Primary context is searched first, then fallback context is searched second if the result from first one was an undefined value.

    .. method:: IContext CreateCustom(System.Func<Value,Value> callback)

        Create a rendering context using given callback for resolving variables. Callback must always expected to return a non-null result, possibly an undefined value.

    .. method:: IContext CreateCustom(System.Collections.Generic.IReadOnlyDictionary<Value,Value> symbols)

        Create a rendering context from given variables dictionary.

    .. method:: ISpyContext CreateSpy(IContext source)

        Wrap given context inside a spying context to get information about variables referenced in a template, along with their last known value and accessed fields. See section :ref:`spy_context` for details about lazy value resolution.

    .. method:: (IContext,ISymbolUsage) CreateMonitor(IContext context)

        Obsolete alternative to :meth:`Context.CreateSpy`.



Function declaration
====================

.. namespace:: Cottle
.. class:: IFunction

    Cottle function interface.

    .. property:: bool IsPure { get; }

        Indicates whether function is pure or not. Pure functions have no side effects nor rely on them, and may offer better rendering performance as they're eligible to more compilation optimizations.

    .. method:: Value Invoke(System.Object state, System.Collections.Generic.IReadOnlyList<Value> arguments, System.IO.TextWriter output)

        Invoke function with given arguments. Variable ``state`` is an opaque payload that needs to be passed to nested function calls if any, ``arguments`` contains the ordered list of values passed to function, and ``output`` is a text writer to document output result.


.. namespace:: Cottle
.. class:: Function

    Methods from this static class must be used to create instances of :type:`IFunction`.

    .. method:: IFunction CreateNativeExact(System.Func<IRuntime,System.Collections.Generic.IReadOnlyList<Value>,System.IO.TextWriter,Value> callback, int count)

        Create a non-pure function accepting exactly ``count`` arguments.

    .. method:: IFunction CreateNativeMinMax(System.Func<IRuntime,System.Collections.Generic.IReadOnlyList<Value>,System.IO.TextWriter,Value> callback, int min, int max)

        Create a non-pure function accepting between ``min`` and ``max`` arguments (included).

    .. method:: IFunction CreateNativeVariadic(System.Func<IRuntime,System.Collections.Generic.IReadOnlyList<Value>,System.IO.TextWriter,Value> callback)

        Create a non-pure function accepting any number of arguments.

    .. method:: IFunction CreateNative0(System.Func<IRuntime,System.IO.TextWriter,Value> callback)

        Create a non-pure function accepting zero argument.

    .. method:: IFunction CreateNative1(System.Func<IRuntime,Value,System.IO.TextWriter,Value> callback)

        Create a non-pure function accepting one argument.

    .. method:: IFunction CreateNative2(System.Func<IRuntime,Value,Value,System.IO.TextWriter,Value> callback)

        Create a non-pure function accepting two arguments.

    .. method:: IFunction CreateNative3(System.Func<IRuntime,Value,Value,Value,System.IO.TextWriter,Value> callback)

        Create a non-pure function accepting three arguments.

    .. method:: IFunction CreatePureExact(System.Func<System.Object,System.Collections.Generic.IReadOnlyList<Value>,Value> callback, int count)

        Create a pure function accepting exactly ``count`` arguments.

    .. method:: IFunction CreatePureMinMax(System.Func<System.Object,System.Collections.Generic.IReadOnlyList<Value>,Value> callback, int min, int max)

        Create a pure function accepting between ``min`` and ``max`` arguments (included).

    .. method:: IFunction CreatePureVariadic(System.Func<System.Object,System.Collections.Generic.IReadOnlyList<Value>,Value> callback)

        Create a pure function accepting any number of arguments.

    .. method:: IFunction CreatePure0(System.Func<System.Object,Value> callback)

        Create a pure function accepting zero argument.

    .. method:: IFunction CreatePure1(System.Func<System.Object,Value,Value> callback)

        Create a pure function accepting one argument.

    .. method:: IFunction CreatePure2(System.Func<System.Object,Value,Value,Value> callback)

        Create a pure function accepting two arguments.

    .. method:: IFunction CreatePure3(System.Func<System.Object,Value,Value,Value,Value> callback)

        Create a pure function accepting three arguments.


.. namespace:: Cottle
.. class:: IRuntime

    Access to runtime execution data.

    .. property:: IMap Globals { get; }

        Global variables defined in template being rendered currently.



Value declaration
=================

.. namespace:: Cottle
.. class:: Value

    Cottle values can hold instances of any of the supported types (see section :ref:`value_types`).

    .. property:: Value EmptyMap { get; }

        Static and read-only empty map value, equal to ``Value.FromEnumerable(Array.Empty<Value>()))``.

    .. property:: Value EmptyString { get; }

        Static and read-only empty string value, equal to ``Value.FromString(string.Empty)``.

    .. property:: Value False { get; }

        Static and read-only boolean "false" value, equal to ``Value.FromBoolean(false)``.

    .. property:: Value True { get; }

        Static and read-only boolean "true" value, equal to ``Value.FromBoolean(true)``.

    .. property:: Value Undefined { get; }

        Static and read-only undefined value, equal to ``new Value()`` or ``default(Value)``.

    .. property:: Value Zero { get; }

        Static and read-only number "0" value, equal to ``Value.FromNumber(0)``.

    .. property:: bool AsBoolean { get; }

        Read value as a boolean after converting it if needed. Following conversion is applied depending on base type:

        * From numbers, return ``true`` for non-zero values and ``false`` otherwise.
        * From strings, return ``true`` for non-zero length values and ``false`` for empty strings.
        * From undefined values, always return ``false``.

    .. property:: IFunction AsFunction { get; }

        Read value as a function, only if base type was already a function. No conversion is applied on this property, and return value is undefined if value was not a function.

    .. property:: double AsNumber { get; }

        Read value as a double precision floating point number after converting it if needed. Following conversion is applied depending on base type:

        * From booleans, return ``0`` for ``false`` or ``1`` for ``true``.
        * From strings, convert to double number if value can be parsed as one using ``double.TryParse()`` on invariant culture, or return ``0`` otherwise.
        * From undefined values, always return ``0``.

    .. property:: string AsString { get; }

        Read value as a string after converting it if needed. Following conversion is applied depending on base type:

        * From booleans, return string ``"true"`` for ``true`` and empty string otherwise.
        * From numbers, return result of call to ``double.ToString()`` method with invariant culture.
        * From undefined values, always return an empty string.

    .. property:: IMap Fields { get; }

        Get child field of current value if any, or an empty map otherwise.

    .. property:: ValueContent Type { get; }

        Get base type of current value instance.

    .. method:: FromBoolean(bool value)

        Create value from given boolean instance.

    .. method:: FromDictionary(System.Collections.Generic.IReadOnlyDictionary<Value,Value> dictionary)

        Create a map value from given keys and associated value in given ``dictionary``, without preserving any ordering. This override assumes input dictionary is immutable and simply keeps a reference on it without duplicating the data structure.

    .. method:: FromEnumerable(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<Value,Value>> pairs)

        Create a map value from given ``elements``, preserving element ordering but also allowing O(1) access to values by key.

    .. method:: FromEnumerable(System.Collections.Generic.IEnumerable<Value> elements)

        Create a map value from given ``elements``. Numeric keys are generated for each element starting at index ``0``.

    .. method:: FromFunction(IFunction function)

        Create a function value by wrapping an executable :type:`IFunction` instance. See sections :ref:`declare_function` and :ref:`native_function` for details about functions in Cottle.

    .. method:: FromGenerator(System.Func<int,Value> generator, int count)

        Create map value from given generator. Generator function ``generator`` is used to create elements based on their index, and the map will contain ``count`` values associated to keys ``0`` to ``count - 1``. Values are created only when retrieved, so creating a generator value with 10000000 elements won't have any cost until you actually access these elements from your template.

    .. method:: FromLazy(System.Func<Value> resolver)

        Create a lazy value from given value resolver. See section :ref:`lazy_value` for details about lazy value resolution.

    .. method:: FromMap(IMap value)

        Create value from given :type:`IMap` instance.

    .. method:: FromNumber(double value)

        Create value from given double instance.

    .. method:: FromReflection<TSource>(TSource source, System.Reflection.BindingFlags bindingFlags)

        Create a reflection-based value to read members from object ``source``. Source object fields and properties are resolved using :meth:`System.Type.GetFields` and :meth:`System.Type.GetProperties` methods and provided binding flags for resolution. See section :ref:`reflection_value` for details about reflection-based inspection.

    .. method:: FromString(string value)

        Create value from given string instance.


.. namespace:: Cottle.Values
.. class:: FunctionValue

    .. inherits:: Cottle.Value

    Deprecated class, use :meth:`Value.FromFunction` to create function values.

    .. method:: FunctionValue(IFunction function)

    Class constructor.


.. namespace:: Cottle.Values
.. class:: LazyValue

    .. inherits:: Cottle.Value

    Deprecated class, use :meth:`Value.FromLazy` to create lazy values.

    .. method:: LazyValue(System.Func<Value> resolver)

    Class constructor.


.. namespace:: Cottle.Values
.. class:: ReflectionValue

    .. inherits:: Cottle.Value

    Deprecated class, use :meth:`Value.FromReflection` to create reflection values.

    .. method:: ReflectionValue(System.Object source, System.Reflection.BindingFlags binding)

    Class constructor with explicit binding flags.

    .. method:: ReflectionValue(System.Object source)

    Class constructor with default binding flags for resolution (public + private + instance).


.. namespace:: Cottle
.. class:: IMap

    Value fields container.

    .. indexer:: Value this[Value key] { get; }

        Get field by its key (usually its name), or an undefined value if no field was defined with this name.

    .. property:: int Count { get; }

        Get number of fields contained within this value.

    .. method:: bool Contains(Value key)

        Check whether current map contains a field with given key or not. Returns ``true`` if map contains requested field or ``false`` otherwise.

    .. method:: bool TryGet(Value key, out Value value)

        Try to read field by key. Returns ``true`` and set output :type:`Value` instance if found, or return ``false`` otherwise.


.. enum:: ValueContent

   Base value type enumeration.

   .. value:: Boolean

        Boolean value, either ``true`` or ``false``.

   .. value:: Function

        Invokable function value.

   .. value:: Map

        Enumerable key/value collection.

   .. value:: Number

        Numeric value, either integer or floating point.

   .. value:: String

        Characters string value.

   .. value:: Void

        Undefined value.



Spying context
==============

.. namespace:: Cottle
.. class:: ISpyContext

    .. inherits:: Cottle.IContext

    Rendering context able to spy on variables and fields used during document rendering.

    .. method:: ISpyRecord SpyVariable(Value key)

        Spy variable matching given key from underlying context. This method can be called either before or after rendering a document, as returned record is updated on each rendering.

    .. method:: System.Collections.Generic.IReadOnlyDictionary<Value,ISpyRecord> SpyVariables()

        Spy all variables used in rendered document from underlying context and return them as a dictionary indexed by variable key. Note that every variable referenced in a document will have an entry in returned dictionary, even if they were not accessed at rendering.


.. namespace:: Cottle
.. class:: ISpyRecord

    Spying information about variable or field value.

    .. property:: Value Value { get; }

        Last value observed at render time for the variable or field being spied on.

    .. method:: ISpyRecord SpyField(Value key)

        Spy field matching given key from current variable or field. This method is similar to :meth:`ISpyContext.SpyVariable` but works on variable fields instead of context variables.

    .. method:: System.Collections.Generic.IReadOnlyDictionary<Value,ISpyRecord> SpyFields()

        Spy all fields from current variable or field and return then as a dictionary indexed by field key.



Exceptions
==========

.. namespace:: Cottle.Exceptions
.. class:: ParseException

    .. inherits:: System.Exception

    Exception class raised when trying to convert an invalid template string into a :type:`IDocument` instance.

    .. property:: string Lexem { get; }

        Lexem (text fragment) that was unexpectedly encountered in template.

    .. property:: int LocationLength { get; }

        Length of the last lexem recognized when encountering a parsing error.

    .. property:: int LocationStart { get; }

        Offset of the last lexem recognized when encountering parsing error.
