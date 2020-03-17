========
Overview
========

.. default-domain:: csharp
.. namespace:: Cottle

What does it looks like?
========================

Cottle (short for **C**\ ompact **O**\ bject to **T**\ ext **T**\ ransform **L**\ anguag\ **e**) is a lightweight template engine for .NET (.NET Framework >= 4.7.2 & .NET Standard >= 2.0) allowing you to transform structured data into any text-based format output such as plain text, HTML or XML. It uses a simple yet easily extensible template language, thus enabling clean separation of document contents and layout.

A simple Cottle template printing an HTML document showing how many messages are in your mailbox could look like this:

.. code-block:: html

    <html>
        <body>
            <h1>Hello, {name}!</h1>
            <p>
                {if len(messages) > 0:
                    You have {len(messages)} new message{if len(messages) > 1:s} in your mailbox!
                |else:
                    You have no new message.
                }
            </p>
        </body>
    </html>

As you can guess by looking at this code, a Cottle template contains both plain text printed as-is as well as commands used to output dynamic contents. Cottle supports most common template engine features, such as:

* Text substitution through variables,
* Mathematical and boolean expressions,
* Built-in and used-defined functions,
* Variables & functions declaration and assignments,
* Conditional statements (if),
* Loops (for, while).

Source code is open for reviews and contributions!



Download the library
====================

Cottle is available as an installable package on `NuGet <https://www.nuget.org/packages/Cottle/>`__ official website. Just open your extension manager in Visual Studio, search for "Cottle" and install from there.

You can also read, download or contribute to the source code on `GitHub <https://github.com/r3c/cottle>`__.



.. _`getting_started`:

Getting started
===============

To start using Cottle, first reference the package in your solution (using NuGet or manual install as detailed above). You'll then need two things:

-  An input template written with Cottle's template language, used to define how your data will be rendered. This template can be contained in a ``string`` or streamed from any source compatible with ``System.IO.TextReader`` class (text file, memory buffer, network socket...) as shown in the example below.
-  An executable code that reads your input template, create a :type:`IDocument` object from it then render it to an output string or ``System.IO.TextWriter`` instance.

Here is a basic sample rendering a template with a single injected variable. Copy the **C# source** snippet somewhere in your program and get it executed. You should see the content of **Rendering output** snippet printed to standard output:

.. code-block:: csharp
    :caption: C# source
    :emphasize-lines: 13

    void RenderAndPrintTemplate()
    {
        var template = "Hello {who}, stay awhile and listen!";

        var documentResult = Document.CreateDefault(template); // Create from template string
        var document = documentResult.DocumentOrThrow; // Throws ParseException on error

        var context = Context.CreateBuiltin(new Dictionary<Value, Value>
        {
            ["who"] = "my friend" // Declare new variable "who" with value "my friend"
        });

        // TODO: customize rendering if needed

        Console.Write(document.Render(context));
    }

.. code-block:: plain
    :caption: Rendering output

    Hello my friend, stay awhile and listen!

For following code samples we'll introduce **Cottle template**, **C# source** and **Rendering output** snippets to hold corresponding fragments. You'll always need a C# wrapper similar to the one above in your code, so only new features will be specified in following examples ; they should replace the **TODO** comment highligted in above **Rendering outout** snippet.
