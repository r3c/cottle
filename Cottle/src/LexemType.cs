using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace   Cottle
{
    enum    LexemType
    {
        None,
        EndOfFile,
        BraceBegin,
        BraceEnd,
        BracketBegin,
        BracketEnd,
        Comma,
        Colon,
        Dot,
        Literal,
        Number,
        Pipe,
        ParenthesisBegin,
        ParenthesisEnd,
        String,
        Text
    }
}
