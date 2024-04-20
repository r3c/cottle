.. default-domain:: csharp
.. namespace:: Cottle

.. _`builtin`:

==================
Built-in functions
==================

Logical
=======

and(x, y, ...)
--------------

Perform logical "and" between given boolean values, i.e. return ``true`` if all arguments are equivalent to ``true`` (see :type:`Value` type for details about conversion to boolean).

.. code-block:: plain
    :caption: Cottle template

    {and(2 < 3, 5 > 1)}

.. code-block:: plain
    :caption: Rendering output

    true

.. note::

    This function is equivalent to operator ``&&``.


.. _`builtin_cmp`:

cmp(x, y)
---------

Compare ``x`` against ``y``, and return -1 if ``x`` is lower than ``y``, 0 if they're equal, or 1 otherwise. When used on numeric values, the ``cmp`` function uses numerical order. When used on strings, it uses alphabetical order. When used on maps, it first performs numerical comparison on their length then compares keys and values two by two. Two values of different types are always different, but the order between them is undefined.

.. code-block:: plain
    :caption: Cottle template

    {cmp("abc", "bcd")}
    {cmp(9, 6)}
    {cmp([2, 4], [2, 4])}

.. code-block:: plain
    :caption: Rendering output

    -1
    1
    0


default(primary, fallback)
--------------------------

Return ``primary`` if ``primary`` is equivalent to ``true`` (see :type:`Value` type for details about conversion to boolean) or ``fallback`` otherwise.

.. code-block:: plain
    :caption: Cottle template

    {set x to 3}
    {default(x, "invisible")}
    {default(y, "visible")}

.. code-block:: plain
    :caption: Rendering output

    3
    visible


defined(x)
--------------

Check whether value ``x`` is defined by checking it has a non-void type.

This is different than checking whether a value is equivalent to ``true`` (see :type:`Value` type for details about conversion to boolean), for example integer ``0`` is equivalent to ``false`` when used as a boolean expression but ``defined(0)`` is ``true``. This function is mostly useful for testing whether a variable has been assigned a value or not.

.. code-block:: plain
    :caption: Cottle template

    {dump defined(undefined)}
    {set a to 0}
    {dump defined(a)}

.. code-block:: plain
    :caption: Rendering output

    <false>
    <true>


eq(x, y, ...)
-------------

Return ``true`` if all arguments are equal or ``false`` otherwise. It uses the same comparison algorithm than function :ref:`builtin_cmp`.

.. code-block:: plain
    :caption: Cottle template

    {eq(7, 7)}
    {eq(1, 4)}
    {eq("test", "test")}
    {eq(1 = 1, 2 = 2, 3 = 3)}

.. code-block:: plain
    :caption: Rendering output

    true
    false
    true
    true

.. note::

    This function is equivalent to operator ``=`` when used with 2 arguments.


ge(x, y)
--------

Return ``true`` if ``x`` has a value greater than or equal to ``y`` or ``false`` otherwise. It uses the same comparison algorithm than function :ref:`builtin_cmp`.

.. code-block:: plain
    :caption: Cottle template

    {ge(7, 3)}
    {ge(2, 2)}
    {ge("abc", "abx")}

.. code-block:: plain
    :caption: Rendering output

    true
    true
    false

.. note::

    This function is equivalent to operator ``>=``.


gt(x, y)
--------

Return ``true`` if ``x`` has a value greater than ``y`` or ``false`` otherwise. It uses the same comparison algorithm than function :ref:`builtin_cmp`.

.. code-block:: plain
    :caption: Cottle template

    {gt(7, 3)}
    {gt(2, 2)}
    {gt("abc", "abx")}

.. code-block:: plain
    :caption: Rendering output

    true
    false
    false

.. note::

    This function is equivalent to operator ``>``.


has(map, key)
-------------

Return ``true`` if given map has a value associated to given key or ``false`` otherwise.

.. code-block:: plain
    :caption: Cottle template

    {has(["name": "Paul", "age": 37, "sex": "M"], "age")}

.. code-block:: plain
    :caption: Rendering output

    true

.. note::

    Result of this function is close to but not strictly equivalent to ``defined(map[key])`` as the former will return ``true`` if ``map`` contains a key ``key`` associated to an undefined value while the later will return ``false``.


le(x, y)
--------

Return ``true`` if ``x`` has a value lower than or equal to ``y`` or ``false`` otherwise. It uses the same comparison algorithm than function :ref:`builtin_cmp`.

.. code-block:: plain
    :caption: Cottle template

    {le(3, 7)}
    {le(2, 2)}
    {le("abc", "abx")}

.. code-block:: plain
    :caption: Rendering output

    true
    true
    true

.. note::

    This function is equivalent to operator ``<=``.


lt(x, y)
--------

Return ``true`` if ``x`` has a value lower than ``y`` or ``false`` otherwise. It uses the same comparison algorithm than function :ref:`builtin_cmp`.

.. code-block:: plain
    :caption: Cottle template

    {lt(3, 7)}
    {lt(2, 2)}
    {lt("abc", "abx")}

.. code-block:: plain
    :caption: Rendering output

    true
    false
    true

.. note::

    This function is equivalent to operator ``<``.


ne(x, y)
-------------

Return ``true`` if ``x`` equals ``y`` or ``false`` otherwise. It uses the same comparison algorithm than function :ref:`builtin_cmp`.

.. code-block:: plain
    :caption: Cottle template

    {ne(7, 7)}
    {ne(1, 4)}
    {ne("test", "test")}

.. code-block:: plain
    :caption: Rendering output

    false true false

.. note::

    This function is equivalent to operator ``!=`` when used with 2 arguments.


not(x)
------

Perform logical "not" on given boolean value, i.e return ``false`` if value was equivalent to ``true`` (see :type:`Value` type for details about conversion to boolean) or ``false`` otherwise.

.. code-block:: plain
    :caption: Cottle template

    {not(1 = 2)}

.. code-block:: plain
    :caption: Rendering output

    true

.. note::

    This function is equivalent to operator ``!``.


or(x, y, ...)
-------------

Perform logical "or" between given boolean values, i.e. return ``true`` if at least one argument is equivalent to ``true`` (see :type:`Value` type for details about conversion to boolean).

.. code-block:: plain
    :caption: Cottle template

    {or(2 = 3, 5 > 1)}

.. code-block:: plain
    :caption: Rendering output

    true

.. note::

    This function is equivalent to operator ``||``.


xor(x, y, ...)
--------------

Perform logical "xor" between given boolean values, i.e. return ``true`` if exactly one argument is ``true`` and all the others are ``false``.

.. code-block:: plain
    :caption: Cottle template

    {xor(2 < 3, 1 = 2)}

.. code-block:: plain
    :caption: Rendering output

    true


when(condition[, truthy[, falsy]])
----------------------------------

Return ``truthy`` if ``condition`` is equivalent to ``true`` (see :type:`Value` type for details about conversion to boolean) or ``falsy`` otherwise (or an undefined value if ``falsy`` is missing). This function is intended to act as the ternary operator you can find in some programming languages.

.. code-block:: plain
    :caption: Cottle template

    {set x to 3}
    {set y to 0}
    {when(x, "x is true", "x is false")}
    {when(y, "y is true", "y is false")}

.. code-block:: plain
    :caption: Rendering output

    x is true
    y is false



Mathematical
============

abs(x)
------------

Return the absolute value of given numeric value ``x``.

.. code-block:: plain
    :caption: Cottle template

    {abs(-3)}
    {abs(5)}

.. code-block:: plain
    :caption: Rendering output

    3
    5


add(x, y)
---------

Return the sum of two numeric values.

.. code-block:: plain
    :caption: Cottle template

    {add(3, 7)}

.. code-block:: plain
    :caption: Rendering output

    10

.. note::

    This function is equivalent to operator ``+``.


ceil(x)
-------

Returns the smallest integer greater than or equal to number value ``x``.

.. code-block:: plain
    :caption: Cottle template

    {ceil(2.7)}

.. code-block:: plain
    :caption: Rendering output

    3


cos(x)
------

Get the cosine of angle ``x`` in radians.

.. code-block:: plain
    :caption: Cottle template

    {cos(-1.57)}

.. code-block:: plain
    :caption: Rendering output

    0.000796326710733263


div(x, y)
---------

Return the numeric value of ``x`` divided by the numeric value of ``y``, or an undefined value if ``y`` was equal to zero.

.. code-block:: plain
    :caption: Cottle template

    {div(5, 2)}

.. code-block:: plain
    :caption: Rendering output

    2.5

.. note::

    This function is equivalent to operator ``/``.


floor(x)
--------

Returns the largest integer less than or equal to number value ``x``.

.. code-block:: plain
    :caption: Cottle template

    {floor(2.7)}

.. code-block:: plain
    :caption: Rendering output

    2


max(x[, y[, z, ...]])
---------------------

Return the highest numeric value among given ones.

.. code-block:: plain
    :caption: Cottle template

    {max(7, 5)}
    {max(6, 8, 5, 7, 1, 2)}

.. code-block:: plain
    :caption: Rendering output

    7
    8

.. note::

    Combine with function :ref:`builtin_call` if you want to get the highest numeric value from an array.


min(x[, y[, z, ...]])
---------------------

Return the lowest numeric value among given ones.

.. code-block:: plain
    :caption: Cottle template

    {min(9, 3)}
    {min(6, 8, 5, 7, 1, 2)}

.. code-block:: plain
    :caption: Rendering output

    3
    1

.. note::

    Combine with function :ref:`builtin_call` if you want to get the lowest numeric value from an array.


mod(x, y)
---------

Return the value of ``x`` modulo ``y``, or an undefined value if ``y`` was equal to zero.

.. code-block:: plain
    :caption: Cottle template

    {mod(7, 3)}

.. code-block:: plain
    :caption: Rendering output

    1

.. note::

    This function is equivalent to operator ``%``.


mul(x, y)
---------

Return the numeric value of ``x`` times ``y``.

.. code-block:: plain
    :caption: Cottle template

    {mul(3, 4)}

.. code-block:: plain
    :caption: Rendering output

    12

.. note::

    This function is equivalent to operator ``*``.


pow(x, y)
---------

Get specified number ``x`` raised to the power ``y``.

.. code-block:: plain
    :caption: Cottle template

    {pow(2, 10)}

.. code-block:: plain
    :caption: Rendering output

    1024


rand([a[, b]])
--------------

Get a pseudo-random numeric value between 0 and 2.147.483.647 inclusive. If numeric ``a`` value is specified, return a pseudo-random numeric value between 0 and ``a`` exclusive. If both numeric values ``a`` and ``b`` are specified, return a pseudo-random numeric value between ``a`` inclusive and ``b`` exclusive.

.. code-block:: plain
    :caption: Cottle template

    {rand()}
    {rand(1, 7)}

.. code-block:: plain
    :caption: Rendering output

    542180393
    5


round(x[, digits])
-----------------------

Rounds number value ``x`` to a specified number of fractional digits ``digits``, or to the nearest integral value if ``digits`` is not specified.

.. code-block:: plain
    :caption: Cottle template

    {round(1.57)}
    {round(1.57, 1)}

.. code-block:: plain
    :caption: Rendering output

    2
    1.6


sin(x)
------

Get the sine of angle ``x`` in radians.

.. code-block:: plain
    :caption: Cottle template

    {sin(1.57)}

.. code-block:: plain
    :caption: Rendering output

    0.999999682931835


sub(x, y)
---------

Return the numeric value of ``x`` minus ``y``.

.. code-block:: plain
    :caption: Cottle template

    {sub(3, 5)}

.. code-block:: plain
    :caption: Rendering output

    -2

.. note::

    This function is equivalent to operator ``-``.



Collection
==========

.. _`builtin_cat`:

cat(a, b, ...)
--------------

Concatenate all input maps or strings into a single one. Keys are **not** preserved when this function used on map values.

.. code-block:: plain
    :caption: Cottle template

    {dump cat("Hello, ", "World!")}
    {dump cat([1, 2], [3])}

.. code-block:: plain
    :caption: Rendering output

    "Hello, World!"
    [1, 2, 3]

.. warning::

    All arguments must share the same type than first one, either map or string.


cross(map1, map2, ...)
----------------------

Return a map containing all pairs from ``map1`` having a key that also exists in ``map2`` and all following maps. Output pair values will always be taken from ``map1``.

.. code-block:: plain
    :caption: Cottle template

    {dump cross([1: "a", 2: "b", 3: "c"], [1: "x", 3: "y"])}

.. code-block:: plain
    :caption: Rendering output

    [1: "a", 3: "c"]


.. _`builtin_except`:

except(map1, map2, ...)
-----------------------

Return a map containing all pairs from ``map1`` having a key that does not exist in ``map2`` and any of following maps. This function can also be used to remove a single pair from a map (if you are sure that it's key is not used by any other pair, otherwise all pairs using that key would be removed also).

.. code-block:: plain
    :caption: Cottle template

    {dump except([1: "a", 2: "b", 3: "c"], [2: "x", 4: "y"])}

.. code-block:: plain
    :caption: Rendering output

    [1: "a", 3: "c"]


find(subject, search[, start])
-----------------------------

Find index of given ``search`` value in a map or sub-string in a string. Returns 0-based index of match if found or -1 otherwise. Search starts at index 0 unless ``start`` argument is specified.

.. code-block:: plain
    :caption: Cottle template

    {find([89, 3, 572, 35, 7], 35)}
    {find("hello, world!", "o", 5)}
    {find("abc", "d")}

.. code-block:: plain
    :caption: Rendering output

    3
    8
    -1


filter(map, predicate[, a, b, ...])
-----------------------------------

Return a map containing all pairs having a value that satisfies given predicate. Function ``predicate`` is invoked for each value from ``map`` with this value as its first argument, and pair is added to output map if predicate result is equivalent to ``true`` (see :type:`Value` type for details about conversion to boolean).

Optional arguments can be specified when calling ``filter`` and will be passed to each invocation of ``predicate`` as second, third, forth argument and so on.

.. code-block:: plain
    :caption: Cottle template

    {dump filter(["a", "", "b", "", "c"], len)}

    {declare multiple_of(x, y) as:
        {return x % y = 0}
    }

    {dump filter([1, 6, 7, 4, 9, 5, 0], multiple_of, 3)}

.. code-block:: plain
    :caption: Rendering output

    ["a", "b", "c"]
    [6, 9, 0]


flip(map)
---------

Return a map were pairs are created by swapping each key and value pair from input map. Using resulting map with the ``for`` command will still iterate through each pair even if there was duplicates, but only the last occurrence of each duplicate can be accessed by key.

.. code-block:: plain
    :caption: Cottle template

    {dump flip([1: "hello,", 2: "world!"])}
    {dump flip(["a": 0, "b": 0])}

.. code-block:: plain
    :caption: Rendering output

    ["hello,": 1, "world!": 2]
    ["a", 0: "b"]


join(map[, string])
-------------------

Concatenate all values from given map pairs, using given string as a separator (or empty string if no separator is provided).

.. code-block:: plain
    :caption: Cottle template

    {join(["2011", "01", "01"], "/")}

.. code-block:: plain
    :caption: Rendering output

    2011/01/01


len(x)
------

Return number of elements in given value, which means the number of pairs for a map or the number of character for a string.

.. code-block:: plain
    :caption: Cottle template

    {len("Hello!")}
    {len([17, 22, 391, 44])}

.. code-block:: plain
    :caption: Rendering output

    6
    4


map(source, modifier[, a, b, ...])
----------------------------------

Return a map where values are built by applying given modifier to map values, while preserving keys. Function ``modifier`` is invoked for each value in ``source`` with this value as its first argument.

Optional arguments can be specified when calling ``map`` and will be passed to each invocation of ``modifier`` as second, third, forth argument and so on.

.. code-block:: plain
    :caption: Cottle template

    {declare square(x) as:
        {return x * x}
    }

    {dump map([1, 2, 3, 4], square)}
    {dump map(["a": 1, "b": 7, "c": 4, "d": 5, "e": 3, "f": 2, "g": 6], lt, 4)}

.. code-block:: plain
    :caption: Rendering output

    [1, 4, 9, 16]
    ["a": 1, "b": 0, "c": 0, "d": 0, "e": 1, "f": 1, "g": 0]


range([start, ]stop[, step])
----------------------------

Generate a map where value of the *i*-th pair is *start + step \* i* and last value is lower (or higher if ``step`` is a negative integer) than ``stop``. Default base index is 0 if the ``start`` argument is omitted, and default value for ``step`` is 1 if ``start`` < ``stop`` or -1 otherwise.

.. code-block:: plain
    :caption: Cottle template

    {for v in range(5): {v}}
    {for v in range(2, 20, 3): {v}}

.. code-block:: plain
    :caption: Rendering output

    0 1 2 3 4
    2 5 8 11 14 17


.. _`builtin_slice`:

slice(subject, index[, count])
------------------------------

Extact sub-string from a string or elements from a map (keys are not preserved when used with maps). ``count`` items or characters are extracted from given 0-based numeric ``index``. If no ``count`` argument is specified, all elements starting from given ``index`` are extracted.

.. code-block:: plain
    :caption: Cottle template

    {for v in slice([68, 657, 54, 3, 12, 9], 3, 2): {v}}
    {slice("abchello", 4)}

.. code-block:: plain
    :caption: Rendering output

    3 12
    hello


sort(map[, callback])
---------------------

Return a sorted copy of given map. First argument is the input map, and will be sorted using natural order (numerical or alphabetical, depending on value types) by default. You can specify a second argument as comparison delegate, that should accept two arguments and return -1 if the first should be placed "before" the second, 0 if they are equal, or 1 otherwise.

.. code-block:: plain
    :caption: Cottle template

    {set shuffled to ["in", "order", "elements" "natural"]}
    {for item in sort(shuffled):
        {item}
    }

    {declare by_length(a, b) as:
        {return cmp(len(b), len(a))}
    }
    {set shuffled to ["by their", "are sorted", "length", "these strings"]}
    {for item in sort(shuffled, by_length):
        {item}
    }

.. code-block:: plain
    :caption: Rendering output

    elements in natural order
    these strings are sorted by their length


.. _`builtin_union`:

union(map1, map2, ...)
----------------------

Return a map containing all pairs from input maps, but without duplicating any key. If a key exists more than once in all input maps, the last one will overwrite any previous pair using it.

.. code-block:: plain
    :caption: Cottle template

    {dump union([1: "a", 2: "b"], [2: "x", 3: "c"], [4: "d"])}

.. code-block:: plain
    :caption: Rendering output

    [1: "a", 2: "x", 3: "c", 4: "d"]


zip(k, v)
---------

Combine given maps of same length to create a new one. The n-th pair in result map will use the n-th value from ``k`` as its key and the n-th value from ``v`` as its value.

.. code-block:: plain
    :caption: Cottle template

    {set k to ["key1", "key2", "key3"]}
    {set v to ["value1", "value2", "value3"]}
    {dump zip(k, v)}

.. code-block:: plain
    :caption: Rendering output

    ["key1": "value1", "key2": "value2", "key3": "value3"]



Text
====

char(codepoint)
---------------

Get a 1-character string from its Unicode code point integer value. See more about Unicode and code points on `Wikipedia <https://en.wikipedia.org/wiki/Unicode>`__.

.. code-block:: plain
    :caption: Cottle template

    {char(97)}
    {char(916)}

.. code-block:: plain
    :caption: Rendering output

    a
    Δ


.. _`builtin_format`:

format(value, format[, culture])
--------------------------------

Convert any ``value`` to a string using given formatting from ``format`` string expression. Format should use syntax ``str`` or ``t:str`` where ``t`` indicates the type of the formatter to use and ``str`` is the associated .NET format string. Available formatter types are:

-  ``a``: automatic (default, used if ``t`` is omitted)
-  ``b``: System.Boolean
-  ``d`` or ``du``: System.DateTime (UTC)
-  ``dl``: System.DateTime (local)
-  ``i``: System.Int64
-  ``n``: System.Double
-  ``s``: System.String

Format string depends on the type of formatter selected, see help about `Format String Component <https://docs.microsoft.com/fr-fr/dotnet/standard/base-types/composite-formatting?view=netframework-4.8#format-string-component>`__ for more information about formats.

.. code-block:: plain
    :caption: Cottle template

    {format(1339936496, "d:yyyy-MM-dd HH:mm:ss")}
    {format(0.165, "n:p2", "fr-FR")}
    {format(1, "b:n2")}

.. code-block:: plain
    :caption: Rendering output

    2012-06-17 12:34:56
    16,50 %
    True

Formatters use current culture, unless a culture name is specified in the ``culture`` argument. See documentation of `CultureInfo.GetCultureInfo <https://docs.microsoft.com/fr-fr/dotnet/api/system.globalization.cultureinfo.getcultureinfo?view=netframework-4.8>`__ method to read more about culture names.


lcase(string)
-------------

Return a lowercase conversion of given string value.

.. code-block:: plain
    :caption: Cottle template

    {lcase("Mixed Case String"}

.. code-block:: plain
    :caption: Rendering output

    mixed case string


match(subject, pattern)
-----------------------

Match ``subject`` against given regular expression pattern. If match is successful, a map containing full match followed by captured groups is returned, otherwise result is an undefined value. See `.NET Framework Regular Expressions <https://docs.microsoft.com/en-us/previous-versions/dotnet/netframework-1.1/hs600312(v=vs.71)?redirectedfrom=MSDN>`__ for more information.

.. code-block:: plain
    :caption: Cottle template

    {dump match("abc123", "^[a-z]+([0-9]+)$")}
    {dump match("xyz", "^[a-z]+([0-9]+)$")}

.. code-block:: plain
    :caption: Rendering output

    ["abc123", "123"]
    <void>


ord(character)
--------------

Get the Unicode code point value of the first character of given string. See more about Unicode and code points on `Wikipedia <https://en.wikipedia.org/wiki/Unicode>`__.

.. code-block:: plain
    :caption: Cottle template

    {ord("a")}
    {ord("Δ")}

.. code-block:: plain
    :caption: Rendering output

    97
    916


split(subject, separator)
-------------------------

Split ``subject`` string according to given string separator ``separator``. Result is an map where pair values contain split sub-strings.

.. code-block:: plain
    :caption: Cottle template

    {dump split("2011/01/01", "/")}

.. code-block:: plain
    :caption: Rendering output

    ["2011", "01", "01"]


token(subject, search, index[, replace])
----------------------------------------

Either return the n-th section of a string delimited by separator substring ``search`` if no ``replace`` argument is provided, or replace this section by ``replace`` else. This function can be used as a faster alternative to combined split/slice/join calls in some cases.

.. code-block:: plain
    :caption: Cottle template

    {token("First.Second.Third", ".", 1)}
    {token("A//B//C//D", "//", 2)}
    {token("XX-??-ZZ", "-", 1, "YY")}
    {token("1;2;3", ";", 3, "4")}

.. code-block:: plain
    :caption: Rendering output

    Second
    C
    XX-YY-ZZ
    1;2;3;4


ucase(string)
-------------

Return an uppercase conversion of given string value.

.. code-block:: plain
    :caption: Cottle template

    {ucase("Mixed Case String"}

.. code-block:: plain
    :caption: Rendering output

    MIXED CASE STRING



Type
====

.. _`builtin_cast`:

cast(value, type)
-----------------

Get value converted to requested scalar type. Type must be a string value specifying desired type:

-  ``"b"`` or ``"boolean"``: convert to boolean value
-  ``"n"`` or ``"number"``: convert to numeric value
-  ``"s"`` or ``"string"``: convert to string value

.. code-block:: plain
    :caption: Cottle template

    {dump cast("2", "n") = 2}
    {dump ["value for key 0"][cast("0", "n")]}
    {dump cast("some string", "b")}

.. code-block:: plain
    :caption: Rendering output

    <true>
    "value for key 0"
    <true>


.. _`builtin_type`:

type(value)
-----------

Retrieve type of given value as a string. Possible return values are ``"boolean"``, ``"function"``, ``"map"``, ``"number"``, ``"string"`` or ``"void"``.

.. code-block:: plain
    :caption: Cottle template

    {type(15)}
    {type("test")}

.. code-block:: plain
    :caption: Rendering output

    number
    string



Dynamic
=======

.. _`builtin_call`:

call(func, map)
---------------

Call function ``func`` with values from ``map`` as arguments (keys are ignored).

.. code-block:: plain
    :caption: Cottle template

    {call(cat, ["Hello", ", ", "World", "!"])}
    {call(max, [3, 8, 2, 7])}

.. code-block:: plain
    :caption: Rendering output

    Hello, World!
    8
