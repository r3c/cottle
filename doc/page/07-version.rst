.. default-domain:: csharp
.. namespace:: Cottle

==========
Versioning
==========

.. _`versioning_convention`:

Versioning convention
=====================

Cottle versioning does **NOT** (exactly) follow SemVer convention but uses closely-related version numbers with form ``MAJOR.MINOR.PATCH`` where:

*  ``MAJOR`` increases when breaking changes are applied and break source compatibility, meaning client code must be changed before it can compile.
*  ``MINOR`` increases when binary compatibility is broken but source compatibility is maintained, meaning client code can be rebuilt with no source change.
*  ``PATCH`` increases when binary compatibility is maintained from previous version, meaning new library version can be used as a drop-in replacement and doesn't require recompiling code.

The main difference between this approach and SemVer is the distinction made between binary compatibility and source compatibility. For example replacing a public field by a property, or doing the opposite, would break strict binary compatibility but wouldn't require any change when recompiling client code unless it's using reflection.



Migration guide
===============

From 1.6.\* to 2.0.\*
---------------------

* Cottle now uses :type:`System.Double` type for number values instead of :type:`System.Decimal` ; use builtin function :ref:`builtin_format` if you need to control decimal precision when printing decimal numbers.
* Type :type:`Value` is now a value type to reduce runtime allocations ; API was upgraded to be source-compatible with previous Cottle versions.
* Specialized value classes (e.g. :type:`Values.FunctionValue`) are deprecated, use ``Value.From*`` static construction methods instead (e.g. :meth:`Value.FromFunction`).

.. code-block:: csharp
    :caption: Example of migration from 1.6.* code to equivalent 2.0.* version

    // Version 1.6.*
    var context = Context.CreateBuiltin(new Dictionary<Value, Value>
    {
        ["f"] = new FunctionValue(myFunction),
        ["n"] = new NumberValue(myNumber)
    };

    // Version 2.0.*
    var context = Context.CreateBuiltin(new Dictionary<Value, Value>
    {
        ["f"] = Value.FromFunction(myFunction),
        ["n"] = Value.FromNumber(myNumber) // Or just `myNumber` to use implicit conversion
    };


From 1.5.\* to 1.6.\*
---------------------

* All documents should be constructed using methods from :type:`Document` static class.
* All contexts should be constructed using methods from :type:`Context` static class.
* All functions should be constructed using methods from :type:`Function` static class.

.. code-block:: csharp
    :caption: Example of migration from 1.5.* code to equivalent 1.6.* version

    // Version 1.5.*
    IDocument document;

    try
    {
        document = new SimpleDocument(template, new CustomSetting
        {
            Trimmer = BuiltinTrimmers.FirstAndLastBlankLines
        });
    }
    catch (ParseException exception)
    {
        MyErrorHandler(exception.Message);

        return string.Empty;
    }

    return document.Render(new BuiltinStore
    {
        ["f"] = new NativeFunction((args, store, output) => MyFunction(args[0].AsNumber, output), 1)
    });

    // Version 1.6.*
    var result = Document.CreateDefault(template, new DocumentConfiguration
    {
        Trimmer = DocumentConfiguration.TrimIndentCharacters
    });

    if (!result.Success)
    {
        MyErrorHandler(result.Reports);

        return string.Empty;
    }

    // Can be replaced by result.DocumentOrThrow to factorize test on "Success" field and use
    // the exception-based API which is closer to what was available in version 1.5.*
    var document = result.Document;

    return document.Render(Context.CreateBuiltin(new Dictionary<Value, Value>
    {
        ["f"] = new FunctionValue(Function.Create1((state, arg, output) => MyFunction(arg.AsNumber, output)))
    });


From 1.4.\* to 1.5.\*
---------------------

*  ``IStore`` replaced by immutable :type:`IContext` interface for rendering documents. Since the former extends the later, migration should only imply recompiling without any code change.
*  Cottle function delegates now receive a ``IReadOnlyList<Value>`` instead of their mutable equivalent.
*  Method ``Save`` from ``DynamicDocument`` can only be used in the .NET Framework version, not the .NET Standard one.


From 1.3.\* to 1.4.\*
---------------------

*  Change of version number convention, breaking source compatibility must now increase major version number.
*  Cottle now requires .NET 4.0 or above.


From 1.2.\* to 1.3.\*
---------------------

*  Removed deprecated code (flagged as "obsolete" in previous versions).


From 1.1.\* to 1.2.\*
---------------------

*  ``IScope`` replaced by similar ``IStore`` interface (they mostly differ by the return type of their "Set" method which made this impossible to change without breaking the API).
*  Callback argument of constructors for ``NativeFunction`` are not compatible with ``IScope`` to avoid ambiguous statements.


From 1.0.\* to 1.1.\*
---------------------

*  ``LexerConfig`` must be replaced by ``CustomSetting`` object to change configuration.
*  ``FieldMap`` has been replaced by multiple implementations of the new ``IMap`` interface.
*  Two values with different types are always different, even if casts could have made them equal (i.e. removed automatic casts when comparing values).
*  Common functions ``cross`` and ``except`` now preserve duplicated keys.
