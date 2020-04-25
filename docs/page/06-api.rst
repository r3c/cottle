=============
API reference
=============

.. default-domain:: csharp
.. namespace:: Cottle

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

    .. method:: Value Render(IContext context, TextWriter writer)

        Render document and write output to given ``TextWriter`` instance. Return value is the value passed to top-level ``return`` command if any, or an undefined value otherwise.

    .. method:: string Render(IContext context)

        Render document and return outout as a ``string`` instance.


.. namespace:: Cottle
.. class:: Document

    Methods from this static class must be used to create instances of :type:`DocumentResult`.

    .. method:: DocumentResult CreateDefault(TextReader template, DocumentConfiguration configuration = default)

        Create a new default :type:`IDocument` instance suitable for most use cases. Template is read from any non-seekable ``TextReader`` instance.

    .. method:: DocumentResult CreateDefault(string template, DocumentConfiguration configuration = default)

        Create a new default :type:`IDocument` instance similar to previous method. Template is read from given ``string`` instance.

    .. method:: DocumentResult CreateNative(TextReader template, DocumentConfiguration configuration = default)

        Create a new native :type:`IDocument` instance for better rendering performance but higher compilation cost. Template is read from any non-seekable ``TextReader`` instance. See section :ref:`native_document` for details about native documents.

    .. method:: DocumentResult CreateNative(string template, DocumentConfiguration configuration = default)

        Create a new native :type:`IDocument` instance similar to previous method. Template is read from given ``string`` instance.


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

        Delimiter for *start of command*, see section :ref:`delimiter_customization` for details.

    .. property:: string BlockContinue { get; set; }

        Delimiter for *continue command*, see section :ref:`delimiter_customization` for details.

    .. property:: string BlockEnd { get; set; }

        Delimiter for *end of command*, see section :ref:`delimiter_customization` for details.

    .. property:: Nullable<char> Escape { get; set; }

        Delimiter for *escape*, see section :ref:`delimiter_customization` for details. Default escape character is **\\** when this property is null.

    .. property:: bool NoOptimize { get; set; }

        Disable code optimizations after compiling a document, see :ref:`optimizer_deactivation` for details.

    .. property:: Func<string,string> Trimmer { get; set; }

        Function used to trim unwanted character out of plain text when parsing a document, see section :ref:`plain_text_trimming` for details.


.. namespace:: Cottle
.. class:: DocumentResult

    This structure holds result of a template compilation, which can either be successful and provide compiled :type:`IDocument` instance or failed and provide compilation error details as a list of ``DocumentReport`` elements:

    .. property:: IDocument Document { get; }

        Instance of compiled document, only if compilation was successful (see :prop:`DocumentResult.Success`).

    .. property:: IReadOnlyList<DocumentReport> Reports { get; }

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

    .. method:: IContext CreateBuiltin(IReadOnlyDictionary<Value,Value> symbols)

        Create a rendering context by combining variables from given dictionary with all Cottle built-in functions. This method is similar to previous one and only exists as a convenience helper.

    .. method:: IContext CreateCascade(IContext primary, IContext fallback)

        Create a rendering context by combining two existing contexts that will be searched in order when querying a variable. Primary context is searched first, then fallback context is searched second if the result from first one was an undefined value.

    .. method:: IContext CreateCustom(Func<Value,Value> callback)

        Create a rendering context using given callback for resolving variables. Callback must always expected to return a non-null result, possibly an undefined value.

    .. method:: IContext CreateCustom(IReadOnlyDictionary<Value,Value> symbols)

        Create a rendering context from given variables dictionary.

    .. method:: (IContext,ISymbolUsage) CreateMonitor(IContext context)

        Wrap given context inside a monitoring context to get statistics on which variables are read from it. Output :type:`IContext` is the monitored one that should be passed to document, while the ``ISymbolUsage`` usage is the structure you can query after rendering the document to get information about which variables were read.



Function declaration
====================

.. namespace:: Cottle
.. class:: IFunction

    Cottle function interface.

    .. property:: bool IsPure { get; }

        Indicates whether function is pure or not. Pure functions have no side effects nor rely on any, and are eligible to various rendering optimizations.

    .. method:: Value Invoke(object state, IReadOnlyList<Value> arguments, TextWriter output)

        Invoke function with given arguments. Variable ``state`` is a opaque payload that needs to be passed to nested function calls if any, ``arguments`` contains the ordered list of values passed to function, and ``output`` is a text writer to document output result.


.. namespace:: Cottle
.. class:: Function

    Methods from this static class must be used to create instances of :type:`IFunction`.

    .. method:: IFunction Create(Func<object,IReadOnlyList<Value>,TextWriter,Value> callback, int min, int max)

        Create a non-pure function accepting between ``min`` and ``max`` arguments (included).

    .. method:: IFunction Create(Func<object,IReadOnlyList<Value>,TextWriter,Value> callback, int count)

        Create a non-pure function accepting exactly ``count`` arguments.

    .. method:: IFunction Create(Func<object,IReadOnlyList<Value>,TextWriter,Value> callback)

        Create a non-pure function accepting any number of arguments.

    .. method:: IFunction Create1(Func<object,Value,TextWriter,Value> callback)

        Create a non-pure function accepting one argument.

    .. method:: IFunction Create2(Func<object,Value,TextWriter,Value> callback)

        Create a non-pure function accepting two arguments.

    .. method:: IFunction CreatePure(Func<object,IReadOnlyList<Value>,Value> callback, int min, int max)

        Create a pure function accepting between ``min`` and ``max`` arguments (included).

    .. method:: IFunction CreatePure(Func<object,IReadOnlyList<Value>,Value> callback, int count)

        Create a pure function accepting exactly ``count`` arguments.

    .. method:: IFunction CreatePure(Func<object,IReadOnlyList<Value>,Value> callback)

        Create a pure function accepting any number of arguments.

    .. method:: IFunction CreatePure1(Func<object,Value,Value> callback)

        Create a pure function accepting one argument.

    .. method:: IFunction CreatePure2(Func<object,Value,Value> callback)

        Create a pure function accepting two arguments.



Value declaration
=================

.. namespace:: Cottle
.. class:: Value

    Cottle values can hold instances of any of the supported types (see section :ref:`value_types`).

    .. property:: Value EmptyMap { get; }

        Static and read-only empty map value.

    .. property:: Value EmptyString { get; }

        Static and read-only empty string value.

    .. property:: Value False { get; }

        Static and read-only boolean "false" value.

    .. property:: Value True { get; }

        Static and read-only boolean "true" value.

    .. property:: Value Undefined { get; }

        Static and read-only undefined value.

    .. property:: Value Zero { get; }

        Static and read-only number "0" value.

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
        * From strings, parse double number if value matches regular expression ``[0-9]*(\\.[0-9]+)?``, or ``0`` otherwise.
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

    .. method:: FromDictionary(IReadOnlyDictionary<Value,Value> dictionary)

        Create a map value from given keys and associated value in given ``dictionary``, without preserving any ordering. This override assumes input dictionary is immutable and simply keeps a reference on it without duplicating the data structure.

    .. method:: FromDictionary(IDictionary<Value,Value> dictionary)

        Create a map value from given keys and associated value in given ``dictionary``, without preserving any ordering. This override assumes input dictionary is mutable and duplicates the data structure to avoid further modifications.

    .. method:: FromEnumerable(IEnumerable<KeyValuePair<Value,Value>> pairs)

        Create a map value from given ``elements``, preserving element ordering but also allowing O(1) access to values by key.

    .. method:: FromEnumerable(IEnumerable<Value> elements)

        Create a map value from given ``elements``. Numeric keys are generated for each element starting at index ``0``.

    .. method:: FromFunction(IFunction function)

        Create a function value by wrapping an executable :type:`IFunction` instance. See sections :ref:`declare_function` and :ref:`native_function` for details about functions in Cottle.

    .. method:: FromGenerator(Func<int,Value> generator, int count)

        Create map value from given generator. Generator function ``generator`` is used to create elements based on their index, and the map will contain ``count`` values associated to keys ``0`` to ``count - 1``. Values are created only when retrieved, so creating a generator value with 10000000 elements won't have any cost until you actually access these elements from your template.

    .. method:: FromLazy(Func<Value> resolver)

        Create a lazy value from given value resolver. See section :ref:`lazy_value` for details about lazy value resolution.

    .. method:: FromMap(IMap value)

        Create value from given :type:`IMap` instance.

    .. method:: FromNumber(double value)

        Create value from given double instance.

    .. method:: FromReflection<TSource>(TSource source, BindingFlags bindingFlags)

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

    .. method:: LazyValue(Func<Value> resolver)

    Class constructor.


.. namespace:: Cottle.Values
.. class:: ReflectionValue

    .. inherits:: Cottle.Value

    Deprecated class, use :meth:`Value.FromReflection` to create reflection values.

    .. method:: ReflectionValue(object source, BindingFlags binding)

    Class constructor with explicit binding flags.

    .. method:: ReflectionValue(object source)

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
