---
layout: default
title: Cottle - High performance & lightweight template library for .NET
---

<div style="text-align: center;">
    <a href="https://travis-ci.org/r3c/cottle">
        <img alt="Build Status" src="https://travis-ci.org/r3c/cottle.svg?branch=master" />
    </a>
    <a href="https://www.nuget.org/packages/Cottle/">
        <img alt="NuGet" src="https://img.shields.io/nuget/v/Cottle.svg" />
    </a>
    <a href="https://opensource.org/licenses/MIT">
        <img alt="License" src="https://img.shields.io/github/license/r3c/cottle.svg" />
    </a>
</div>


Overview
========

Cottle is an open-source (MIT) templating engine for C# .NET designed to be
light (no external dependency & simple API), fast (see
[benchmark](./benchmark.html)) and extensible (see
[advanced features](https://cottle.readthedocs.io/en/stable/page/05-advanced.html)).
Cottle language and C# API look like this:

```sh
{library} is a great library to {["discover", "learn", "use"][rand(0, 3)]}!
```

```cs
var document = Document.CreateDefault(template).DocumentOrThrow;

return document.Render(Context.CreateBuiltin(new Dictionary<Value, Value>
{
    ["library"] = "Cottle"
}));
```

```
Cottle is a great library to learn!
```


Getting started
===============

[User documentation](https://cottle.readthedocs.io/) is available at Read the
Docs and explains how to use the library with many code snippets and examples.

[Performance benchmark](./benchmark.html) against similar libraries shows how
Cottle performs compared to other equivalent template engines.


License
=======

This project is open-source, released under MIT licence. See
[license page](https://github.com/r3c/cottle/blob/master/license.md) for
details.
