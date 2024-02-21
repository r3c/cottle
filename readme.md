# Cottle: Compact Object to Text Transform Language

[![Build Status](https://img.shields.io/github/actions/workflow/status/r3c/cottle/verify.yml?branch=master)](https://github.com/r3c/cottle/actions/workflows/verify.yml)
[![NuGet](https://img.shields.io/nuget/v/Cottle.svg)](https://www.nuget.org/packages/Cottle/)
[![license](https://img.shields.io/github/license/r3c/cottle.svg)](https://opensource.org/licenses/MIT)

## Overview

Cottle is an open-source (MIT) templating engine for C# .NET designed to be
light (no external dependency & simple API), fast (see
[benchmark](https://r3c.github.io/cottle/benchmark.html)) and extensible (see
[advanced features](https://cottle.readthedocs.io/en/stable/page/05-advanced.html)).

## Sample

```sh
Hello, {name}!

{if len(messages) > 0:
    You have {len(messages)} new message{if len(messages) > 1:s} in your mailbox!
|else:
    You have no new message.
}
```

```cs
var document = Document.CreateDefault(template).DocumentOrThrow;

return document.Render(Context.CreateBuiltin(new Dictionary<Value, Value>
{
    ["messages"] = GetMessages(),
    ["name"] = "JC Denton"
}));
```

## Resource

- Home page: https://r3c.github.io/cottle/
- Documentation: https://cottle.readthedocs.io/
- Benchmark: https://r3c.github.io/cottle/benchmark.html
- NuGet package: https://www.nuget.org/packages/Cottle
- Contact: v.github.com+cottle [at] mirari [dot] fr
- License: [license.md](license.md)
