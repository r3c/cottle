TODO list
=========

TODO
----

- Implement compile-time variable resolution for `NativeDocument`
- New syntax with {{ }} default separators & multiple statements support
- Value type replacement of `Value` class
- Sample `include` function available in documentation

DONE
----

- Use pool for allocated locales (Cottle.Documents.Dynamic.Compiler)
- Fix scope enter/leave issue with "return" keyword (Cottle.Documents.Dynamic.Compiler)
- Optimize string and value storage lookup (Cottle.Documents.Dynamic.Compiler)
- Write "return" optimizer (Cottle.Parsers.Post.Optimizers.ReturnOptimizer)
- Reduce .maxstack size in dynamic documents (Cottle.Documents.Dynamic.Compiler)
- Value type replacement of `ISetting` interface
