TODO list
=========

TODO
----

- Implement new "RemoveIndent" builtin trimmer to replace "TrimIndentCharacters"
- Use fixed arguments instead of arguments array in `EmittedDocument`
- Leverage `FiniteFunction` for defined functions with 4 arguments or less
- New syntax with {{ }} default separators & multiple statements support
- Provide sample `include` function in documentation

DONE
----

- Use pool for allocated locals (Cottle.Documents.Dynamic.Compiler)
- Fix scope enter/leave issue with "return" keyword (Cottle.Documents.Dynamic.Compiler)
- Optimize string and value storage lookup (Cottle.Documents.Dynamic.Compiler)
- Write "return" optimizer (Cottle.Parsers.Post.Optimizers.ReturnOptimizer)
- Reduce .maxstack size in dynamic documents (Cottle.Documents.Dynamic.Compiler)
- Value type replacement of `ISetting` interface
- Implement compile-time variable resolution for `NativeDocument`
- Value type replacement of `Value` class
