.. default-domain:: csharp
.. namespace:: Cottle

=================
Template language
=================

Language syntax
===============

Plain text and commands
-----------------------

A Cottle template can contain plain text printed as-is as well as code blocks containing commands that will be executed when document is rendered. These commands can either print dynamic content or have side-effects such as defining variables or controlling the rendering flow.

The most important command you'll need is the ``echo`` command that takes an argument and outputs its contents. Here is how it works:

.. code-block:: plain
    :caption: Cottle template

    Value of x is {echo x}.

.. code-block:: csharp
    :caption: C# source

    var context = Context.CreateBuiltin(new Dictionary<Value, Value>
    {
        ["x"] = 53
    });

.. code-block:: plain
    :caption: Rendering output

    Value of x is 53.

In this example we're creating a *variable* named ``x`` with value 53 and pass it when rendering our template, then we're using the ``echo`` command to print the value of this variable. As you can tell the part between ``{`` and ``}`` is a code block containing executable commands, while everything else is plain text that is copied to document output.


Block delimiters
----------------

All commands must be wrapped in a code block between ``{`` (*block begin*) and ``}`` (*block end*) delimiters, which can be redefined in configuration if needed (read section :ref:`delimiter_customization` to learn how). Delimiters must be escaped if you want to use them in plain text, otherwise they would be misinterpreted as delimiters. This can be achieved by using ``\\`` (*escape*) delimiter as shown below:

.. code-block:: plain
    :caption: Cottle template

    Characters \{, \}, \| and \\ must be escaped when used in plain text.

.. code-block:: plain
    :caption: Rendering output

    Characters {, }, | and \ must be escaped when used in plain text.

As visible in this example, backslash character ``\\`` must also be used to escape itself when you want to output a backslash. Similar to other delimiters, the *escape* delimiter can be redefined through configuration.


Implicit echo
-------------

Since ``echo`` is the most frequent command it supports a shorter implicit form where the "echo" keyword can be omitted:

.. code-block:: plain
    :caption: Cottle template

    Value of x is {x}.

Implicit form of ``echo`` command can be used everywhere as long as you're not printing a variable having the same name than a Cottle command such as ``for``. While technically possible, using Cottle command names as variables should be avoided for readability reasons anyway.



Expressions
===========

Passing variables
-----------------

To send variables so they can be used when a document is rendered you must provide them through a :type:`IContext` instance which is used as a render-time and read-only storage. This interface behaves quite like a ``IReadOnlyDictionary<Cottle.Value, Cottle.Value>`` where :type:`Value` is a data structure able to store any value Cottle can handle. Key and value pairs within this dictionary are used as variable names and their associated values.

Implicit constructors from some native .NET types to :type:`Value` type are provided so you usually don't have to explicitly do the conversion yourself but you can also create values using ``Value.FromSomething()`` static construction methods (where "Something" is a known .NET type). See API documentation about :type:`Value` type for details.

Once you assigned variables to a context, pass it to your document's rendering method so you can read them from your template (see section :ref:`getting_started` for a full example):

.. code-block:: plain
    :caption: Cottle template

    Hello {name}, you have no new message.

.. code-block:: csharp
    :caption: C# source

    var context = Context.CreateBuiltin(new Dictionary<Value, Value>
    {
        ["name"] = "John" // Implicit conversion from string on both key and value
    });

.. code-block:: plain
    :caption: Rendering output

    Hello John, you have no new message.

Instances of :type:`IContext` are passed at document render time so they can be changed from one render to another, while instances of :type:`IDocument` can then be rendered as many time as you want. Compiling a template string into an :type:`IDocument` is a costly process implying parsing the string, validating its contents, applying code optimizations and storing it as an internal data structure. You should organize your code to avoid re-creating documents from the same template multiple time, as compiling a document is significantly more costly than rendering it.


.. _`value_types`:

Value types
-----------

Cottle supports immutable values which can either be declared as constants in templates or set in contexts you pass when rendering a document. Values have a type which can be one of the following:

-  Boolean (value is either true or false),
-  Number (equivalent to .NET's double),
-  String (sequence of character),
-  Map (associative key/value container),
-  Void (value is undefined ; any undeclared variable has void type).

Map values are associative tables that contain multiple children values stored as key/value pairs. Values within a map can be accessed directly by their key, using either dotted or subscript notation:

.. code-block:: plain
    :caption: Cottle template

    You can use either {mymap.f1} or {mymap["f2"]} notations for map values.

.. code-block:: csharp
    :caption: C# source

    var context = Context.CreateBuiltin(new Dictionary<Value, Value>
    {
        ["mymap"] = new Dictionary<Value, Value> // Implicit conversion to Value
        {
            ["f1"] = "dotted",
            ["f2"] = "subscript"
        }
    });

.. code-block:: plain
    :caption: Rendering output

    You can use either dotted or subscript notations for map values.

Please note the quotes used in subscript notation. Trying to access value of ``{mymap[f2]}`` will result in a very different behavior, since it will search for the value whose key is the value of ``f2`` (which hasn't be defined), leading to an undefined result. It is valid to have a map in which two or more keys are equal, but you will only be able to access the last one when using direct access. Iterating over the map's elements will however show you its entire contents.

Implicit constructors on :type:`Value` class allow you to convert most .NET standard types into a Cottle value instance. To get an undefined value your from C# code use the ``Cottle.Value.Undefined`` static field.

You can also declare constant values in your templates with following constructs:

.. code-block:: plain
    :caption: Cottle template

    {17.42}
    {"Constant string"}
    {'String with single quotes'}
    {["key1": "value1", "key2": "value2"]}
    {["map", "with", "numeric", "keys"]}

When declaring a constant map without keys, numeric increasing keys (starting at index 0) are implied. Also remember that both keys and values can be of any value type (numbers, strings, other nested maps, etc.).

.. note::

    There are no `false` nor `true` constants in Cottle. You can inject them as variables if needed, but numeric values 0 and 1 can be considered as equivalent in most scenarios.


Expression operators
--------------------

Cottle supports common mathematical and logical operators. Here is the list of all operators sorted by decreasing precedence order:

-  ``+``, ``-`` and ``!``: unary plus, minus and logical "not" operator ;
-  ``*``, ``/`` and ``%``: binary multiplication, division and modulo operators ;
-  ``+`` and ``-``: binary addition and subtraction operators ;
-  ``<``, ``<=``, ``=``, ``!=``, ``>=`` and ``>``: binary logical comparison operators ;
-  ``&&`` and ``||``: binary "and" and "or" logical operators.

You can also use ``(`` and ``)`` to group sub-expressions and change natural precedence order. Here are some example of valid expressions:

.. code-block:: plain
    :caption: Cottle template

    {1 + 2 * 3}
    {(1 + 2) * 3}
    {!(x < 1 || x > 9)}
    {value / 2 >= -10}
    {"aaa" < "aab"}

.. note::

    Mathematical operators (``+``, ``-``, ``*``, ``/`` and ``%``) only accept numeric operands and will try to cast other types to numbers (see :type:`Value` type for details about conversion to number).

.. note::

    Logical operators can compare any type of operand and uses the same comparison algorithm than built-in function :ref:`builtin_cmp`.


Calling functions
-----------------

Functions in Cottle are special values that can be invoked with arguments specified between a pair of parenthesis and separated by commas. Functions must be registered in a context as any other value type, and a helper method is available so you can start with a predefined set of built-in functions when rendering your documents. Create a context using :meth:`Context.CreateBuiltin` method to have all built-in functions available in your document:

.. code-block:: plain
    :caption: Cottle template

    You have {len(messages)} new message{when(len(messages) > 1, 's')} in your inbox.

.. code-block:: csharp
    :caption: C# source

    var context = Context.CreateBuiltin(new Dictionary<Value, Value>
    {
        ["messages"] = new Value[]
        {
            "message #0",
            "message #1",
            "message #2"
        }
    });

.. code-block:: plain
    :caption: Rendering output

    You have 3 new messages in your inbox.

The list of all built-in functions as well as their behavior is available in section :ref:`builtin`. For all following samples in this document we'll assume that built-in functions are available when rendering a template.

.. note::

    If you don't want any built-in function to be available in your template, you can start off with a blank context by calling :meth:`Context.CreateCustom` method.



Commands
========


.. _`command_wrap`:

Text escaping: wrap & unwrap
----------------------------

*Added in version 2.0.0*

You'll most probably want to escape unsafe values (e.g. user input) before printing their contents from your templates, like making sure characters "<" and ">" are replaced by "&lt;" and "&gt;" when printing variables to an HTML document.

While this can be achieved by injecting an escaping function (e.g. :type:`System.Web.HttpUtility.HtmlEncode`) and call it on every expression you pass to ``echo`` command, a nice alternative is using ``wrap`` command to ensure nothing is left unescaped before printing:

.. code-block:: html
    :caption: Cottle template

    {wrap html:
        <p data-description="{op_description}">
            {op_name}
        </p>
    }

.. code-block:: csharp
    :caption: C# source

    var htmlEncode = Function.CreatePure1((s, v) => HttpUtility.HtmlEncode(v.AsString));
    var context = Context.CreateBuiltin(new Dictionary<Value, Value>
    {
        ["html"] = Value.FromFunction(htmlEncode),
        ["op_description"] = "Three-way comparison or \"spaceship operator\"",
        ["op_name"] = "<=>"
    });

.. code-block:: html
    :caption: Rendering output

    <p data-description="Three-way comparison or &quot;spaceship operator&quot;">
        &lt;=&gt;
    </p>

The ``wrap`` command syntax is ``{wrap function:some {body} here}`` where ``function`` is a function expression and the part between ``:`` (*body declaration*) and ``}`` (*block end*) delimiters is template code. The template code enclosed by ``wrap`` command will have ``function`` invoked on the expression of every ``echo`` command it contains to modify its value before it gets printed. This means our previous example will produce an output equivalent to this template:

.. code-block:: html
    :caption: Cottle template

    <p data-description="{html(op_description)}">
        {html(op_name)}
    </p>

You may occasionally want to cancel wrapping for printing a safe HTML snippet without wrapping it. This can be achieved with the ``unwrap`` command that cancels its parent ``wrap`` command:

.. code-block:: html
    :caption: Cottle template

    {wrap html:
        <p>This {variable} will be HTML-escaped.</p>
        {unwrap:
            <p>This {raw} one won't so make sure it doesn't contain unvalidated user input!</p>
        }
        <p>We're back in {safe} context here with HTML escaping enabled.</p>
    }

Multiple ``wrap`` commands can be nested, resulting in their functions being called from the innermost to outermost ``wrap`` command.


.. _`command_if`:

Conditionals: if
----------------

You can write conditional statements by using the ``if`` command which uses an expression as a predicate to check whether its body should be printed or not. Predicate is verified if value, once converted to a boolean type, is true (see :type:`Value` type for details about conversion to boolean).

.. code-block:: plain
    :caption: Cottle template

    {if 1:
        A condition on a numeric value is true if the value is non-zero.
    }

    {if "aaa":
        {if 1 + 1 = 2:
            Commands can be nested.
        }
    }

.. code-block:: plain
    :caption: Rendering output

    A condition on a numeric value is true if the value is non-zero.

    Commands can be nested.

The ``if`` command syntax, similarly to ``wrap`` command, is ``{if condition:when {condition} is true}`` where ``condition`` is a predicate expression and the part between ``:`` (*body declaration*) and ``}`` (*block end*) delimiters is template code. It also supports optional ``elif`` (else if) and ``else`` blocks that behave like in most programming languages, using syntax ``{if first:X|elif second:Y|else:Z}``. Both ``elif`` and ``else`` commands must be preceeded by a ``|`` (*block continue*) delimiter.

.. code-block:: plain
    :caption: Cottle template

    {if len(items) > 2:
        There are more than two items in map ({len(items)}, actually).
    }

    {if test:
        Variable "test" is true!
    |else:
        Variable "test" is false!
    }

    {if x < 0:
        X is negative.
    |elif x > 0:
        X is positive.
    |else:
        X is zero.
    }

.. code-block:: csharp
    :caption: C# source

    var context = Context.CreateBuiltin(new Dictionary<Value, Value>
    {
        ["items"] = new Value[]
        {
            "item #0",
            "item #1",
            "item #2"
        },
        ["test"] = 42,
        ["x"] = -3
    });

.. code-block:: plain
    :caption: Rendering output

    There are more than two items in map (3, actually).

    Variable "test" is true!

    X is negative.


Enumerations: for
-----------------

Keys and values within a map can be enumerated using the ``for`` command, which repeatedly evaluates its body for each key/value pair contained within the map. The ``for`` command also supports an optional ``empty`` block evaluated when the map you enumerated doesn't contain any key/value pair.

The ``for`` command syntax and the one of its optional ``empty`` block are similar to ``if`` and ``else`` commands (see section :ref:`command_if`):

.. code-block:: plain
    :caption: Cottle template

    Tags for this album:
    {for tag in tags:
        {tag}
    }

    {for index, text in messages:
        Message #{index + 1}: {text}
    |empty:
        No messages to display.
    }

.. code-block:: csharp
    :caption: C# source

    var context = Context.CreateBuiltin(new Dictionary<Value, Value>
    {
        ["messages"] = new Value[]
        {
            "Hi, this is a sample message!",
            "Hi, me again!",
            "Hi, guess what?"
        },
        ["tags"] = new Value[]
        {
            "action",
            "horror",
            "fantastic"
        }
    });

.. code-block:: plain
    :caption: Rendering output

    Tags for this album: action horror fantastic

    Message #1: Hi, this is a sample message!
    Message #2: Hi, me again!
    Message #3: Hi, guess what?

.. note::

    Use syntax ``for value in map`` instead of ``for key, value in map`` if you don't need to use map keys.


.. _`command_set`:

Assignments: set
----------------

You can assign variables during rendering with the ``set`` command. Variable assignment helps you improving performance by storing intermediate results (such as function calls) when using them multiple times.

.. code-block:: plain
    :caption: Cottle template

    {set nb_msgs to len(messages)}

    {if nb_msgs > 0:
        You have {nb_msgs} new message{if nb_msgs > 1:s} in your mailbox!
    |else:
        You have no new message.
    }

    {set nb_long to 0}

    {for message in messages:
        {if len(message) > 20:
            {set nb_long to nb_long + 1}
        }
    }

    {nb_long} message{if nb_long > 1:s are|else: is} more than 20 characters long.

.. code-block:: csharp
    :caption: C# source

    var context = Context.CreateBuiltin(new Dictionary<Value, Value>
    {
        ["messages"] = new Value[]
        {
            "Hi, this is a sample message!"
            "Hi, me again!",
            "Hi, guess what?"
        }
    });

.. code-block:: plain
    :caption: Rendering output

    You have 3 new messages in your mailbox!

    1 message is more than 20 characters long.

.. note::

    Cottle variables have visibility scopes, which are described in section :ref:`variable_scope`.


Loops: while
------------

The ``while`` command evaluates a predicate expression and continues executing its body until predicate becomes false. Be sure to check for a condition that will become false after a finite number of iterations, otherwise rendering of your template may never complete.

.. code-block:: plain
    :caption: Cottle template

    {set min_length to 64}
    {set result to ""}
    {set words to ["foo", "bar", "baz"]}

    {while len(result) < min_length:
        {set result to cat(result, words[rand(len(words))])}
    }

    {result}

.. code-block:: plain
    :caption: Rendering output

    barbazfoobarbazbazbazbarbarbarbarfoofoofoobarfoobazfoofoofoofoobaz

.. warning::

    Prefer the use of the ``for`` command over ``while`` command whenever possible, as the former provides better protection against infinite loops.


.. _`command_dump`:

Debug: dump
-----------

When your template doesn't render as you would expect, the ``dump`` command can help you identify issues by showing value as an explicit human readable string. For example undefined values won't print anything when passed through the ``echo`` command, but the ``dump`` command will show them as ``<void>``.

.. code-block:: plain
    :caption: Cottle template

    {dump "string"}
    {dump 42}
    {dump unknown(3)}
    {dump [856, "hello", "x": 17]}

.. code-block:: plain
    :caption: Rendering output

    "string"
    42
    <void>
    [856, "hello", "x": 17]

.. note::

    Command ``dump`` is a debugging command. If you want to get type of a value in production code, see :ref:`builtin_type` method.


Comments: _
-----------

You can use the ``_`` (underscore) command to add comments to your template. This command can be followed by an arbitrary plain text and will be stripped away when template is rendered.

.. code-block:: plain
    :caption: Cottle template

    {_ This is a comment that will be ignored when rendering the template}
    Hello, World!

.. code-block:: plain
    :caption: Rendering output

    Hello, World!


Chaining
--------

*Added in version 2.0.7*

Multiple commands can be chained using to the ``|`` (*block continue*) delimiter. This delimiter can replace the ``}`` (*end of command*) delimiter of any command to issue multiple commands without having to close and open new code blocks. In other words this allow writing ``{set x to 5 | echo x}`` instead of ``{set x to 5}{echo x}``, which helps keeping your code easier to read by letting you indent it as you like without producing unwanted whitespace characters in output result (since whitespaces inside a code block are ignored).

.. code-block:: plain
    :caption: Cottle template

    {
        _ Compute x to the power n using exponentiation by squaring |
        declare power(x, n) as:{
            declare m as 1 |
            while n > 1:{
                if n % 2 = 0:{
                    set x to x * x |
                    set n to n / 2
                } |
                else:{
                    set m to m * x |
                    set n to n - 1
                }
            } |
            return m * x
        } |
        power(2, 5)
    }

.. code-block:: plain
    :caption: Rendering output

    32
