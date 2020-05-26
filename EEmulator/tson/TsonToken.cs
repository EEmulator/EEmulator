////////////////////////////////////////////////////////////////////////////////////
//    MIT License
//
//    Copyright (c) 2020 Atilla Lonny (https://github.com/atillabyte/tson)
//
//    Permission is hereby granted, free of charge, to any person obtaining a copy
//    of this software and associated documentation files (the "Software"), to deal
//    in the Software without restriction, including without limitation the rights
//    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//    copies of the Software, and to permit persons to whom the Software is
//    furnished to do so, subject to the following conditions:
//
//    The above copyright notice and this permission notice shall be included in all
//    copies or substantial portions of the Software.
//
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//    SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////

using Superpower.Display;
using Superpower.Parsers;

namespace Tson
{
    // The parser is token-based. This enum lists the various kinds
    // of element that make up a TSON document. The token kinds map very
    // closely to what you see in the boxes of the syntax diagrams.
    enum TsonToken
    {
        // In general, it's better to be more, rather than less-specific
        // when it comes to choosing what kinds of tokens to generate,
        // because conditional rules in the parser - like "match this
        // kind of token" - can be written more simply if the tokens are
        // descriptive.
        [Token(Example = "{")]
        LBracket,

        // The `Token` attribute lets a little more information be
        // associated with the token. In error messages, the token will
        // normally be described by lower-casing the enum variant name:
        // "unexpected rbracket" - the `Example` property will turn this
        // into "unexpected `}`".
        [Token(Example = "}")]
        RBracket,

        // Notice that the tokens describe the characters and clumps of
        // characters in the language - it's a "bracket", at this level, not
        // an "array start".
        [Token(Example = "[")]
        LSquareBracket,

        [Token(Example = "]")]
        RSquareBracket,

        [Token(Example = ":")]
        Colon,

        [Token(Example = ",")]
        Comma,

        PropertyString,

        // This is a legacy type; it is the exact same as a JSON number.
        Number,

        ByteArray,
        Integer,
        String,
        UInteger,
        Boolean,
        Char,
        Byte,
        SByte,
        Short,
        UShort,
        Long,
        ULong,
        Double,
        Float,
        DateTime,
        Null,

        // Although TSON doesn't have an "identifier" or "keyword"
        // concept that groups `true`, `false`, and `null`, it's useful
        // for the tokenizer to be very permissive - it's more informative
        // to generate an error later at the parsing stage, e.g.
        // "unexpected identifier `false`", instead of failing at the
        // tokenization stage where all we'd have is "unexpected `l`".
        Identifier,
    }
}
