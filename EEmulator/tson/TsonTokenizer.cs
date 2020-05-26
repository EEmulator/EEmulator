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

using System.Linq;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace Tson
{
    // The tokenizer here is assembled using `TokenizerBuilder`. The `Instance`
    // property is the place to start reading, but it has to be ordered below
    // the two "recognizers" that it depends upon, or else they'll be uninitialized
    // when we try to put the tokenizer together.
    static class TsonTokenizer
    {
        // This is a "recognizer" that matches - just like a regular expression would -
        // a block of input text that resembles a TSON string. Notice that it's very
        // permissive - it's only concerned with finding the start and end of the string,
        // and doesn't try verifying the correctness of escape sequences and so-on.
        //
        // The `Unit` type is just a way of expressing `Void` in a C#-friendly way: the
        // recognizer doesn't need to return a value, just match the text.
        //
        // The parser uses built-ins like `Character.EqualTo()` and `Span.EqualTo()`.
        // There's a whole range of simple parsers like these in the `Parsers` namespace.
        //
        // Also in here, you encounter a couple of Superpower's less-obvious combinators:
        //
        //  * `Try()` causes the attempted match of `\"` to backtrack; in order to report
        //    errors accurately, Superpower fails fast when a parser partially matches its
        //    input, as `Span.EqualTo("\\\"")` will when it finds other escape sequences
        //    such as `\n`. When we backtrack, we can try parsing the `\` again with the
        //    following `Character.Except('"')` parser.
        //  * `Value()` here is just being used cast the span parser and the character
        //    parser to compatible types (since we don't care about the values matched
        //    by either of them.
        //  * `IgnoreMany()` is an optimization - we could have used `Many()` here, only
        //    that would allocate an array to return its value - `IgnoreMany()` just
        //    drops the items that it matches.

        static TextParser<Unit> TsonPropertyStringToken { get; } =
            from open in Character.EqualTo('"')
            from content in Span.EqualTo("\\\"").Value(Unit.Value).Try()
                .Or(Character.Except('"').Value(Unit.Value))
                .IgnoreMany()
            from close in Character.EqualTo('"')
            select Unit.Value;

        static TextParser<Unit> TsonByteArrayToken { get; } =
            from open in Span.EqualTo("bytes(\"")
            from content in Span.EqualTo("\\\"").Value(Unit.Value).Try()
                .Or(Character.Except('"').Value(Unit.Value))
                .IgnoreMany()
            from close in Span.EqualTo("\")")
            select Unit.Value;

        static TextParser<Unit> TsonStringToken { get; } =
            from open in Span.EqualTo("string(\"")
            from content in Span.EqualTo("\\\"").Value(Unit.Value).Try()
                .Or(Character.Except('"').Value(Unit.Value))
                .IgnoreMany()
            from close in Span.EqualTo("\")")
            select Unit.Value;

        static TextParser<Unit> TsonIntegerToken { get; } =
            from open in Span.EqualTo("int(")
            from content in Character.In(new[] { '-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }).Many().Value(Unit.Value).Try()
            from close in Span.EqualTo(")")
            select Unit.Value;

        static TextParser<Unit> TsonUIntegerToken { get; } =
            from open in Span.EqualTo("uint(")
            from content in Character.Digit.Many().Value(Unit.Value).Try()
            from close in Span.EqualTo(")")
            select Unit.Value;

        static TextParser<Unit> TsonBooleanToken { get; } =
            from open in Span.EqualTo("bool(")
            from content in Span.EqualTo("true").Or(Span.EqualTo("false")).Value(Unit.Value).Try()
            from close in Span.EqualTo(")")
            select Unit.Value;

        static TextParser<Unit> TsonCharToken { get; } =
            from open in Span.EqualTo("char(\"")
            from content in Span.EqualTo("\\\"").Value(Unit.Value).Try()
                .Or(Character.Except('"').Value(Unit.Value))
                .IgnoreMany()
            from close in Span.EqualTo("\")")
            select Unit.Value;

        static TextParser<Unit> TsonByteToken { get; } =
            from open in Span.EqualTo("byte(")
            from content in Character.In(new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }).Many().Value(Unit.Value).Try()
            from close in Span.EqualTo(")")
            select Unit.Value;

        static TextParser<Unit> TsonSByteToken { get; } =
            from open in Span.EqualTo("sbyte(")
            from content in Character.In(new[] { '-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }).Many().Value(Unit.Value).Try()
            from close in Span.EqualTo(")")
            select Unit.Value;

        static TextParser<Unit> TsonShortToken { get; } =
            from open in Span.EqualTo("short(")
            from content in Character.In(new[] { '-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }).Many().Value(Unit.Value).Try()
            from close in Span.EqualTo(")")
            select Unit.Value;

        static TextParser<Unit> TsonUShortToken { get; } =
            from open in Span.EqualTo("ushort(")
            from content in Character.Digit.Many().Value(Unit.Value).Try()
            from close in Span.EqualTo(")")
            select Unit.Value;

        static TextParser<Unit> TsonLongToken { get; } =
            from open in Span.EqualTo("long(")
            from content in Character.In(new[] { '-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }).Many().Value(Unit.Value).Try()
            from close in Span.EqualTo(")")
            select Unit.Value;

        static TextParser<Unit> TsonULongToken { get; } =
            from open in Span.EqualTo("ulong(")
            from content in Character.Digit.Many().Value(Unit.Value).Try()
            from close in Span.EqualTo(")")
            select Unit.Value;

        static TextParser<Unit> TsonFloatToken { get; } =
            from open in Span.EqualTo("float(")
            from sign in Character.EqualTo('-').OptionalOrDefault()
            from first in Character.Digit
            from rest in Character.Digit.Or(Character.In('.', 'e', 'E', '+', '-')).IgnoreMany()
            from close in Span.EqualTo(")")
            select Unit.Value;

        static TextParser<Unit> TsonDoubleToken { get; } =
            from open in Span.EqualTo("double(")
            from sign in Character.EqualTo('-').OptionalOrDefault()
            from first in Character.Digit
            from rest in Character.Digit.Or(Character.In('.', 'e', 'E', '+', '-')).IgnoreMany()
            from close in Span.EqualTo(")")
            select Unit.Value;

        static TextParser<Unit> TsonDateTimeToken { get; } =
            from open in Span.EqualTo("datetime(\"")
            from content in Span.EqualTo("\\\"").Value(Unit.Value).Try()
                .Or(Character.Except('"').Value(Unit.Value))
                .IgnoreMany()
            from close in Span.EqualTo("\")")
            select Unit.Value;

        static TextParser<Unit> TsonNullToken { get; } =
            from open in Span.EqualTo("null(")
            from close in Span.EqualTo(")")
            select Unit.Value;

        // Like the string parser, the number version is permissive - it's just looking 
        // for a chunk of input that looks something like a JSON number, and not
        // necessarily a valid one.
        static TextParser<Unit> LegacyNumberToken { get; } =
            from sign in Character.EqualTo('-').OptionalOrDefault()
            from first in Character.Digit
            from rest in Character.Digit.Or(Character.In('.', 'e', 'E', '+', '-')).IgnoreMany()
            select Unit.Value;

        // Here's the tokenizer. Working through the input text, the rules are
        // tried in top-to-bottom order until one of them matches. When a rule
        // matches, that rule's token will be produced, and then the tokenizer
        // starts again with the remaining input.
        //
        // The number and identifier tokens are marked as requiring delimiters
        // (all of the other tokens/whitespace are implicitly considered to
        // be delimiters), so that, say, "123abc" isn't inadvertently tokenized
        // as a number, "123", followed by the identifier "abc". The `requireDelimiters`
        // flag deals with this case.
        //
        // One important note about the tokenizer - it's not the place to detect or report
        // errors, except when it's unavoidable. Better errors can be generated later
        // in the parsing process. That's why we accept anything that looks remotely
        // like an identifier, and only check during parsing whether it's a
        // `true`, `false`, `null`, or some invalid junk.
        public static Tokenizer<TsonToken> Instance { get; } =
            new TokenizerBuilder<TsonToken>()
                .Ignore(Span.WhiteSpace)
                .Match(Character.EqualTo('{'), TsonToken.LBracket)
                .Match(Character.EqualTo('}'), TsonToken.RBracket)
                .Match(Character.EqualTo(':'), TsonToken.Colon)
                .Match(Character.EqualTo(','), TsonToken.Comma)
                .Match(Character.EqualTo('['), TsonToken.LSquareBracket)
                .Match(Character.EqualTo(']'), TsonToken.RSquareBracket)
                .Match(TsonPropertyStringToken, TsonToken.PropertyString)
                .Match(LegacyNumberToken, TsonToken.Number, requireDelimiters: true)
                .Match(TsonStringToken, TsonToken.String)
                .Match(TsonByteArrayToken, TsonToken.ByteArray)
                .Match(TsonIntegerToken, TsonToken.Integer)
                .Match(TsonUIntegerToken, TsonToken.UInteger)
                .Match(TsonBooleanToken, TsonToken.Boolean)
                .Match(TsonCharToken, TsonToken.Char)
                .Match(TsonByteToken, TsonToken.Byte)
                .Match(TsonSByteToken, TsonToken.SByte)
                .Match(TsonShortToken, TsonToken.Short)
                .Match(TsonUShortToken, TsonToken.UShort)
                .Match(TsonLongToken, TsonToken.Long)
                .Match(TsonULongToken, TsonToken.ULong)
                .Match(TsonFloatToken, TsonToken.Float)
                .Match(TsonDoubleToken, TsonToken.Double)
                .Match(TsonDateTimeToken, TsonToken.DateTime)
                .Match(TsonNullToken, TsonToken.Null)
                .Match(Identifier.CStyle, TsonToken.Identifier, requireDelimiters: true)
                .Build();
    }
}
