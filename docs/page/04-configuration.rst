.. default-domain:: csharp
.. namespace:: Cottle

======================
Compiler configuration
======================

Specifying configuration
========================

You can specify configuration parameters by passing a :type:`DocumentConfiguration` instance when creating a new document. Here is how to specify configuration parameters:

.. code-block:: csharp
    :caption: C# source
    :emphasize-lines: 5,10

    void RenderAndPrintTemplate()
    {
        var configuration = new DocumentConfiguration
        {
            NoOptimize = true
        };

        var template = "This is my input template file";

        var documentResult = Document.CreateDefault(template, configuration);

        // TODO: render document
    }

Options can be set by assigning a value to optional fields of structure :type:`DocumentConfiguration`, as described below. Any undefined field will keep its default value.



.. _`plain_text_trimming`:

Plain text trimming
===================

Cottle's default behavior when rendering plain text is to output it without any modification. While this gives you a perfect character-level control of how a template is rendered, it may prevent you from writing clean indented code for target formats where whitespaces are not meaningful, such as HTML or JSON.

For this reason you can change the way plain text is transformed through the use of text *trimmers*. A text trimmer is a simple ``Func<string, string>`` function that takes a plain text value and returns it as it should be written to output. Some default trimmer functions are provided by Cottle, but you can inject any custom function you need as well.


TrimEnclosingWhitespaces
------------------------

:prop:`DocumentConfiguration.TrimEnclosingWhitespaces` removes all leading and trailing blank characters from plain text blocks. You may need to use expression ``{' '}`` to force insertion of whitespaces between blocks:

.. code-block:: plain
    :caption: Cottle template

    {'white'}    {'spaces '} around plain    text    blocks {'will'}{' '}{'be'} coll    {'apsed'} .

.. code-block:: csharp
    :caption: C# source
    :emphasize-lines: 3

    var configuration = new DocumentConfiguration
    {
        Trimmer = DocumentConfiguration.TrimEnclosingWhitespaces
    };

.. code-block:: plain
    :caption: Rendering output

    whitespaces around plain    text    blocks will be collapsed.


TrimFirstAndLastBlankLines
--------------------------

*Added in version 2.0.2*

:prop:`DocumentConfiguration.TrimFirstAndLastBlankLines` removes end of line followed by blank characters at beginning and end of plain text blocks. You may have to introduce two line breaks instead of one when interleaving plain text and code blocks so one of them is preserved, or use ``{" "}`` to force some whitespaces at the beginning or end of plain text blocks.

.. code-block:: plain
    :caption: Cottle template

    You have {len(messages)} message
    {if len(messages) > 1:
        s
    }
    {" "}in your inbox.

    I can force

    {"line breaks"}

    to appear.

.. code-block:: csharp
    :caption: C# source
    :emphasize-lines: 3

    var configuration = new DocumentConfiguration
    {
        Trimmer = DocumentConfiguration.TrimFirstAndLastBlankLines
    };

.. code-block:: plain
    :caption: Rendering output

    You have 4 messages in your inbox.

    I can force
    line breaks
    to appear.

.. note::

    This trimmer is used by default when no configuration is specified.


TrimNothing
-----------

:prop:`DocumentConfiguration.TrimNothing` doesn't changing anything on plain text blocks:

.. code-block:: plain
    :caption: Cottle template

    {'no'} change {'will'}
        be applied
    {'on'} plain {'text'} blocks.

.. code-block:: csharp
    :caption: C# source
    :emphasize-lines: 3

    var configuration = new DocumentConfiguration
    {
        Trimmer = DocumentConfiguration.TrimNothing
    };

.. code-block:: plain
    :caption: Rendering output

    no change will
        be applied
    on plain text blocks.


TrimRepeatedWhitespaces
-----------------------

:prop:`DocumentConfiguration.TrimRepeatedWhitespaces` replaces all sequences of white characters (spaces, line breaks, etc.) by a single space, similar to what HTML or XML languages do:

.. code-block:: plain
    :caption: Cottle template

    <ul>    {for s in ["First", "Second", "Third"]:    <li>    {s} </li>    } </ul>

.. code-block:: csharp
    :caption: C# source
    :emphasize-lines: 3

    var configuration = new DocumentConfiguration
    {
        Trimmer = DocumentConfiguration.TrimRepeatedWhitespaces
    };

.. code-block:: plain
    :caption: Rendering output

    <ul>  <li> First </li>  <li> Second </li>  <li> Third </li>  </ul>



.. _`delimiter_customization`:

Delimiters customization
========================

Default Cottle configuration uses ``"{"`` character as *block begin* delimiter, ``"|"`` as *block continue* delimiter and ``"}"`` as *block end* delimiter. These characters may not be a good choice if you want to write a template that would often use them in plain text context, for example if you're writing a JavaScript template, because you would have to escape every **{**, **}** and **|** to avoid Cottle seeing them as delimiters.

A good solution to this problem is changing default delimiters to replace them by more convenient sequences for your needs. Any string can be used as a delimiter as long as it doesn't conflict with a valid Cottle expression (e.g. ``"["``, ``"+"`` or ``"<"``). Make sure at least the first character of your custom delimiters won't cause any ambiguity when choosing them, as the compilation error messages you may have would be confusing.

Default escape delimiter **\\** can be replaced in a similar way, however it must be a single-character value.

.. code-block:: plain
    :caption: Cottle template

    Delimiters are {{block_begin}}, {{block_continue}} and {{block_end}}.
    Backslash \ is not an escape character.

.. code-block:: csharp
    :caption: C# source
    :emphasize-lines: 3,4,5,6

        var configuration = new DocumentConfiguration
        {
            BlockBegin = "{{",
            BlockContinue = "{|}",
            BlockEnd = "}}",
            Escape = '\0'
        };

        var context = Context.CreateBuiltin(new Dictionary<Value, Value>
        {
            ["block_begin"] = "double left brace (" + configuration.BlockBegin + ")"
            ["block_continue"] = "brace pipe brace (" + configuration.BlockContinue + ")",
            ["block_end"] = "double right brace (" + configuration.BlockEnd + ")"
        });

.. code-block:: plain
    :caption: Rendering output

    Delimiters are double left brace ({{), brace pipe brace ({|}) and double right brace (}}).
    Backslash \ is not an escape character.



.. _`optimizer_deactivation`:

Optimizer deactivation
======================

Cottle performs various code optimizations on documents after parsing them from a template to achieve better rendering performance. These optimizations have an additional cost at compilation, which you may not want to pay if you're frequently re-building document instances (which is something you should avoid if possible):

.. code-block:: csharp
    :caption: C# source
    :emphasize-lines: 3

    var configuration = new DocumentConfiguration
    {
        NoOptimize = true
    };

.. warning::

    Disabling optimizations is not recommended for production usage.



Compilation reports
===================

The :type:`DocumentResult` structure returned after compiling a document contains information about any issue detected from input template along with their criticity level (see :type:`DocumentSeverity`), even though only `Error` ones prevent the document from being built. These issues can be accessed like this:

.. code-block:: csharp
    :caption: C# source
    :emphasize-lines: 3,4,5,6

    var documentResult = Document.CreateDefault(template, configuration);

    for (var report in documentResult.Reports)
    {
        Console.WriteLine($"[{report.Severity}] {report.Message}");
    }

Reports can be logged somewhere so you receive notifications whenever an issue is detected in your templates or a migration is suggested.

.. note::

    The `DocumentOrThrow` helper from :type:`DocumentResult` will throw if reports contains one or more item with `Error` criticity level, and use the message from this item as the exception message.



.. _`native_document`:

Native documents
================

You can use "native" documents instead of default ones to achieve better rendering performance at a higher compilation cost. Native documents rely on IL code generation instead of runtime evaluation, and can provide a rendering performance boost from 10% to 20% depending on templates and environment (see `benchmark <https://r3c.github.io/cottle/benchmark.html>`__). They're however two to three times most costly to build, so this feature should be used only when you need high rendering performances on long-lived documents.

To create native documents, simply invoke :meth:`Document.CreateNative` instead of default method:

.. code-block:: csharp
    :caption: C# source

    var document = Document.CreateNative(template).DocumentOrThrow;
