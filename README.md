Cottle: Compact Object to Text Transform Language
=================================================

Overview
--------

Cottle is an open-source (BSD) template engine designed to be light, efficient
and extensible. If you never used a template engine before, here is a
simple example about how it could be used:

- A program of yours generated some structured data, for example a list of
products with associated name, description, quantity and price for each of
them.
- You want to make out an invoice from this list, meaning you want to render
them as a formatted output like an HTML page, an XML document or a plain text
file.
- By writing a template, which is a file that describes how your inputs should
be rendered to produce the final invoice, Cottle allows you to manage this data
transform. Once your template is ready, it can be re-used with any products
list as long as it keeps the same structure (to keep with our example, products
with names, descriptions, quantities and prices).

Template files must be written using Cottle's specific template language, as
explained in the user manual (see "Installation" section).

Installation
------------

This repository contains a Visual Studio 2010 C# solution with two projects:

- Cottle, the library itself, which is the project you'll include in your own
solution if you want to use it. It is compatible with .NET 3.5 and above.
- Demo, a simple Windows Form test program that allows you to write templates
directly in a text box and render them using predefined data.

User manual (the "manual.html" at repository root, also available through
http://r3c.github.com/cottle/) explains how to use the library, and contains a
lot of code examples.

Licence
-------

This project is open-source, released under BSD licence. See "LICENSE" file for
more information. Any contribution would be of course highly welcomed!

Author
------

Rémi Caput (cottle [at] mirari [dot] fr)
http://remi.caput.fr/

If you use my library, please drop me a line!
