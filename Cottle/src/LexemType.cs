using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace   Cottle
{
    enum    LexemType
    {
        EOF,
        BRACE_BEGIN,
        BRACE_END,
        BRACKET_BEGIN,
        BRACKET_END,
        COMMA,
        COLON,
        DOT,
        LITERAL,
        NUMBER,
        PIPE,
        PARENTHESIS_BEGIN,
        PARENTHESIS_END,
        STRING,
        TEXT
    }
}
