Cottle: Compact Object to Text Transform Language
=================================================

[![Build Status](https://travis-ci.org/r3c/cottle.svg?branch=master)](https://travis-ci.org/r3c/cottle)

Overview
--------

Cottle is an open-source (MIT) templating engine for C# .NET designed to be
light, fast and extensible. If you never used a templating engine before, here
is a simple example about how it could be used:

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

Template files must be written using Cottle's template language as explained in
user manual (see "Installation" section).

Installation
------------

Cottle can be installed from Nuget into your project using following command:
<pre>
Install-Package Cottle
</pre>

This repository contains a Visual Studio 2010 C# solution with two projects:

- Cottle, the library itself, which is the project you'll include in your own
solution if you want to use it. It is compatible with .NET 4.0 and above.
- Cottle.Demo, a simple Windows Form test program that allows you to write
templates directly in a text box and render them using predefined data.

User manual is available at http://r3c.github.io/cottle/ (or in `gh-pages`
branch of this repository) and explains how to use the library with many code
snippets and examples.

Licence
-------

This project is open-source, released under MIT licence. See `LICENSE.md` file
for details.

Author
------

[Rémi Caput](http://remi.caput.fr/) (github.com+cottle [at] mirari [dot] fr)
