.. default-domain:: csharp
.. namespace:: Cottle

=================
Advanced features
=================

Understanding value types
=========================

Every value has a type in Cottle, even if you usually don't have to worry about it (see :type:`Value` type for details). Functions that expect arguments of specific types will try to cast them silently and fallback to undefined values when they can't. However in some rare cases you may have to force a cast yourself to get desired result, for example when accessing values from a map:

.. code-block:: plain
    :caption: Cottle template

    {set map to ["first", "second", "third"]}
    {set key to "1"}
    {dump map[key]}

.. code-block:: plain
    :caption: Rendering output

    <void>

You could have expected this template to display "second", but Cottle actually searches for the map value associated to key ``"1"`` (as a string), not key ``1`` (as a number). These are two different values and storing two different values for keys ``"1"`` and ``1`` in a map is valid, hence no automatic cast can be performed for you.

In this example, you can explicitly change the type of ``key`` to a number by using built-in function :ref:`builtin_cast`. Also remember you can use the :ref:`command_dump` command to troubleshoot variable types in your templates.



Variables immutability
======================

All variables in Cottle are immutable, meaning it's not possible to replace a section within a string or change the value associated to some key in a map. If you want to append, replace or erase a value in a map you'll have to rebuild a new one where you inject, filter out or replace desired value. There are a few built-in functions you may find handy to achieve such tasks:

* :ref:`builtin_cat` and :ref:`builtin_union` can merge strings (``cat`` only) or maps ;
* :ref:`builtin_slice` can extract part of a string or a map ;
* :ref:`builtin_except` can extract the intersection between two maps.

Here are a few examples about how to use them:

.. code-block:: plain
    :caption: Cottle template

    {set my_string to "Modify me if you can"}
    {set my_string to cat("I", slice(my_string, 16), ".")}
    {dump my_string}

    {set my_array to [4, 8, 50, 90, 23, 42]}
    {set my_array to cat(slice(my_array, 0, 2), slice(my_array, 4))}
    {set my_array to cat(slice(my_array, 0, 2), [15, 16], slice(my_array, 2))}
    {dump my_array}

    {set my_hash to ["delete_me": "TODO: delete this value", "let_me": "I shouldn't be touched"]}
    {set my_hash to union(my_hash, ["append_me": "I'm here!"])}
    {set my_hash to except(my_hash, ["delete_me": 0])}
    {dump my_hash}

.. code-block:: plain
    :caption: Rendering output

    "I can."
    [4, 8, 15, 16, 23, 42]
    ["let_me": "I shouldn't be touched", "append_me": "I'm here!"]



.. _`declare_function`:

Function declaration
====================

Cottle allows you to declare functions directly in your template code so you can reuse code as you would do with any other programming language. To declare a function and assign it to a variable, use the same ``set`` command you used for regular values assignments (see section :ref:`command_set`) with a slightly different syntax. Function arguments must be specified between parenthesis right after the variable name that should receive the function, and the ``to`` keyword must be followed by a ":" (semicolon) character, then function body declaration as a Cottle template.

Functions can return a value that can be used in any expression or stored in a variable. To make a function halt and return a value, use the ``return`` command within its body:

.. code-block:: plain
    :caption: Cottle template

    {set factorial(n) to:
        {if n > 1:
            {return n * factorial(n - 1)}
        |else:
            {return 1}
        }
    }

    Factorial 1 = {factorial(1)}
    Factorial 3 = {factorial(3)}
    Factorial 8 = {factorial(8)}

    {set hanoi_recursive(n, from, by, to) to:
        {if n > 0:
            {hanoi_recursive(n - 1, from, to, by)}
            Move one disk from {from} to {to}
            {hanoi_recursive(n - 1, by, from, to)}
        }
    }

    {set hanoi(n) to:
        {hanoi_recursive(n, "A", "B", "C")}
    }

    {hanoi(3)}

.. code-block:: plain
    :caption: Rendering output

    Factorial 1 = 1
    Factorial 3 = 6
    Factorial 8 = 40320

    Move one disk from A to C
    Move one disk from A to B
    Move one disk from C to B
    Move one disk from A to C
    Move one disk from B to A
    Move one disk from B to C
    Move one disk from A to C

You can see in this example that returning a value and printing text are two very different things. Plain text within function body is printed each time the function is called, or more precisely each time its enclosing block is executed (that means it won't print if contained in an ``if`` command that fails to pass, for example).

The value returned by the function won't be printed unless you explicitly require it by using the ``echo`` command (e.g. something like ``{factorial(8)}``). If a function doesn't use any ``return`` command it returns an undefined value, that's why the call to ``{hanoi(3)}`` in the sample above does not print anything more than the plain text blocks it contains.



.. _`variable_scope`:

Variable scope
==============

When writing complex templates using nested or recursive functions, you may have to take care of variable scopes to avoid potential issues. A scope is the local evaluation context of any function or command having a body. When assigning a value to a variable (see section :ref:`command_set` for details) all variables belong to the same global scope. Consider this template:

.. code-block:: plain
    :caption: Cottle template

    {set depth(item) to:
        {set res to 0}

        {for child in item:
            {set res_child to depth(child) + 1}
            {set res to max(res, res_child)}
        }

        {return res}
    }

    {depth([["1.1", "1.2", ["1.3.1", "1.3.2"]], "2", "3", ["4.1", "4.2"]])}

The ``depth`` function is expected to return the level of the deepest element in a value that contains nested maps. Of course it could be written in a more efficient way without using non-necessary temporary variables, but it would hide the problem we want to illustrate. If you try to execute this code you'll notice it returns ``2`` where ``3`` would have been expected.

Here is the explanation: when using the ``set`` method to assign a value to variable ``res`` it always uses the same ``res`` instance. The ``depth`` function recursively calls itself but overwrite the unique ``res`` variable each time it tries to store a value in it, and therefore fails to store the actual deepest level as it should.

To solve this issue, the ``res`` variable needs to be local to function ``depth`` so that each invocation uses its own ``res`` instance. This can be achieved by using the ``declare`` command that creates a variable in current scope. Our previous example can then be fixed by declaring a new ``res`` variable inside body of function ``depth``, so that every subsequent reference to ``res`` resolves to our local instance:

.. code-block:: plain
    :caption: Cottle template
    :emphasize-lines: 2

    {set depth(item) to:
        {declare res}
        {set res to 0}

        {for child in item:
            {set res_child to depth(child) + 1}
            {set res to max(res, res_child)}
        }

        {return res}
    }

    {depth([["1.1", "1.2", ["1.3.1", "1.3.2"]], "2", "3", ["4.1", "4.2"]])}

You could even optimize the first ``set`` command away by assigning a value to ``res`` during declaration ; the ``declare`` command actually supports the exact same syntax than ``set``, the only difference being than "to" should be replaced by "as":

.. code-block:: plain
    :caption: Cottle template

    {declare res as 0}

The same command can also be used to declare functions:

.. code-block:: plain
    :caption: Cottle template

    {declare square(n) as:
        {return n * n}
    }

Note that the ``set`` command can also be used without argument, and assigns variable an undefined value (which is equivalent to reset it to an undefined state).



.. _`native_function`:

Native .NET functions
=====================

If you need new features or improved performance, you can assign your own .NET methods to template variables so they're available as Cottle functions. That's actually what Cottle does when you use :meth:`Context.CreateBuiltin` method: a set of Cottle methods is added to your context, and you can have a look at the source code to see how these methods work.

To pass a function in a context, use one of the methods from :type:`Function` class, then pass it to :meth:`Value.FromFunction` method to wrap it into a value you can add to a context:

.. code-block:: plain
    :caption: Cottle template

    Testing custom "repeat" function:

    {repeat("a", 15)}
    {repeat("oh", 7)}
    {repeat("!", 10)}

.. code-block:: csharp
    :caption: C# source

    var context = Context.CreateBuiltin(new Dictionary<Value, Value>
    {
        ["repeat"] = Value.CreateFunction(Function.CreatePure2((state, subject, count) =>
        {
            var builder = new StringBuilder();

            for (var i = 0; i < count; ++i)
                builder.Append(subject);

            return builder.ToString();
        }))
    });

.. code-block:: plain
    :caption: Rendering output

    Testing custom "repeat" function:

    aaaaaaaaaaaaaaa
    ohohohohohohoh
    !!!!!!!!!!

Static class :type:`Function` supports multiple methods to create Cottle functions. Each method expects a .NET callback that contains the code to be executed when the method is invoked, and some of them also ask for the accepted number of parameters for the function being defined. Methods from :type:`Function` are defined across a combination of 2 criteria:

* Whether they're having side effects or not:
    * Methods :meth:`Function.CreatePure`, :meth:`Function.CreatePure1` and :meth:`Function.CreatePure2` must be pure functions having no side effect and not relying on anything but their arguments. This assumption is used by Cottle to perform optimizations in your templates. For this reason their callbacks don't receive a ``TextWriter`` argument as pure methods are not allowed to write anything to output.
    * Methods :meth:`Function.Create`, :meth:`Function.Create1` and :meth:`Function.Create2` are allowed to perform side effects but will be excluded from most optimizations. Their callbacks receive a ``TextWriter`` argument so they can write any text contents to it.
* How many arguments they accept:
    * Methods :meth:`Function.Create` and :meth:`Function.CreatePure` with no integer argument accept any number of arguments, it is the responsibility of provided callback to validate this number.
    * Methods :meth:`Function.Create` and :meth:`Function.CreatePure` with a ``count`` integer accept exactly this number of arguments or return an undefined value otherwise.
    * Methods :meth:`Function.Create` and :meth:`Function.CreatePure` with two ``min`` and ``max`` integers accept a number of arguments contained between these two values or return an undefined value otherwise.
    * Methods :meth:`Function.CreateN` and :meth:`Function.CreatePureN` only accept exactly ``N`` arguments or return an undefined value otherwise.

The callback you'll pass to :type:`Function` takes multiple arguments:

* First argument is always an internal state that must be forwarded to any nested function call ;
* Next arguments are either a list of values (for functions accepting variable number of arguments) or separate scalar values (for functions accepting a fixed number of arguments) received as arguments when invoking the function ;
* Last argument, for non-pure functions only, is a ``TextWriter`` instance open to current document output.



.. _`lazy_value`:

Lazy value evaluation
=====================

In some cases, you may want to inject to your template big and/or complex values that may or may not be needed at rendering, depending on other parameters. In such configurations, it may be better to avoid injecting the entire value in your context if there is chances it won't be used, and use lazy evaluation instead.

Lazy evaluation allows you to inject a value with a resolver callback which will be called only the first time value is accessed, or not called at all if value is not used for rendering. Lazy values can be created through implicit conversion from any ``Func<Value>`` instance or by using :meth:`Value.FromLazy` construction method:

.. code-block:: plain
    :caption: Cottle template

    {if is_admin:
        Administration log: {log}
    }

.. code-block:: csharp
    :caption: C# source
    :emphasize-lines: 4

    var context = Context.CreateBuiltin(new Dictionary<Value, Value>
    {
        ["is_admin"] = user.IsAdmin,
        ["log"] = () => log.BuildComplexLogValue() // Implicit conversion to lazy value
    });

    document.Render(context, Console.Out);

In this example, method ``log.BuildComplexLogValue`` won't be called unless ``is_admin`` value is true.



.. _`reflection_value`:

Reflection values
=================

Instead of converting complex object hierarchies to Cottle values, you can have the library do it for you by using .NET reflection. Keep in mind that reflection is significantly slower than creating Cottle values manually, but as it's a lazy mechanism it may be a good choice if you have complex objects and don't know in advance which fields might be used in your templates.

To use reflection, invoke :meth:`Value.FromReflection` method on any .NET object instance and specify binding flags to indicate which members should be made visible to Cottle. Fields and properties resolved on the object will be accessible like if it were a Cottle map:

.. code-block:: plain
    :caption: Cottle template

    Your image has a size of {image.Width}x{image.Height} pixels.

    {for key, value in image:
        {if value:
            {key} = {value}
        }
    }

.. code-block:: csharp
    :caption: C# source
    :emphasize-lines: 3

    var context = Context.CreateBuiltin(new Dictionary<Value, Value>
    {
        ["image"] = Value.FromReflection(new Bitmap(50, 50), BindingFlags.Instance | BindingFlags.Public)
    });

.. code-block:: plain
    :caption: Rendering output

    Your image has a size of 50x50 pixels.

    Width = 50
    Height = 50
    HorizontalResolution = 96
    VerticalResolution = 96
    Flags = 2

.. warning::

    Relying on reflection has a significant impact on execution performance. Use this feature only if performance is not important for your application, or you don't have other option like explicitly converting fields and properties to a Cottle value.



.. _`spy_context`:

Spying values
=============

If you're working with many templates you may lose track of what variables are used and how. This is where the spying feature can come handy: it allows gathering information on each variable referenced in a template and their associated values. To use this feature, start by wrapping a context within a spying context using the :meth:`Context.CreateSpy` method and use it for rendering your documents:

.. code-block:: csharp
    :caption: C# source

    var spyContext = Context.CreateSpy(regularContext);
    var output = document.Render(spyContext);

    var someVariable = spyContext.SpyVariable("one"); // Access information about variable "one"
    var someVariableField = someVariable.SpyField("field"); // Access map value field information

    var allVariables = spyContext.SpyVariables(); // Access information about all template variables

Read about interface :type:`ISpyContext` for more information about how to use spying context methods.

.. warning::

    Spying context has a negative impact on both performance and memory usage. You may want to apply some sampling strategy if you need to enable this feature in production.
