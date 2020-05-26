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

using System;
using System.Linq;
using Superpower;
using Superpower.Parsers;

namespace Tson
{
    // By this point, we have everything we need to break a TSON document down
    // into a stream of tokens resembling `[LBracket, String("foo"), Colon, ...]`.
    //
    // From this point there are actually two jobs remaining to do.
    // 
    // We'll need to assemble the structure of the TSON document, with arrays,
    // objects, and so-on, and we'll also have to decode strings and
    // numbers into their .NET representations - `string`s and `double`s.
    //
    // Before we assemble the document, we'll jump to the latter task: the
    // `TsonTextParsers` class contains two standalone character-driven parsers
    // that can handle the (non-trivial) string and number formats from
    // the TSON spec.
    static class TsonTextParsers
    {
        // Text parsers like these use the same `TextParser<T>` type that the
        // recognizers in the tokenizer use. This might get a little confusing,
        // but it's also rather handy: these parsers could just as easily do
        // double duty as recognizers in the tokenizer - we don't do that here
        // because we want the tokenizer to be more permissive (and there's
        // some redundant work done for things like decoding escape
        // sequences and allocating the .NET `string` object that the tokenizer
        // can avoid).
        //
        // Most of the parsers and combinators here are self-explanatory, but
        // we also encounter a `Span` parser being used in conjunction with
        // `Apply()` to glue together some pre-built parsers to deal with the
        // four-character unicode character code escape (like "\u0056"). This part
        // could be written a few different ways, along the lines of:
        //
        // `Character.HexDigit.Repeat(4).Select(chs => (char)int.Parse(new string(chs), ...`
        //
        // In general, using Superpower parsers over methods like `int.Parse()` will
        // be less likely to throw exceptions, and should report better error info
        // just in case some invalid input slips through.
        //
        // `Named()` is used here to insert text into "expectations"; rather than
        // reporting "expected `\`, `"`, `/`, `b`, f`.." etc., the error message
        // generated for an invalid escape sequence reads "expected escape sequence".
        public static TextParser<string> PropertyString { get; } =
            from open in Character.EqualTo('"')
            from chars in Character.ExceptIn('"', '\\')
                .Or(Character.EqualTo('\\')
                    .IgnoreThen(
                        Character.EqualTo('\\')
                        .Or(Character.EqualTo('"'))
                        .Or(Character.EqualTo('/'))
                        .Or(Character.EqualTo('b').Value('\b'))
                        .Or(Character.EqualTo('f').Value('\f'))
                        .Or(Character.EqualTo('n').Value('\n'))
                        .Or(Character.EqualTo('r').Value('\r'))
                        .Or(Character.EqualTo('t').Value('\t'))
                        .Or(Character.EqualTo('u').IgnoreThen(
                                Span.MatchedBy(Character.HexDigit.Repeat(4))
                                    .Apply(Numerics.HexDigitsUInt32)
                                    .Select(cc => (char)cc)))
                        .Named("escape sequence")))
                .Many()
            from close in Character.EqualTo('"')
            select new string(chars);

        public static TextParser<byte[]> ByteArray { get; } =
            from open in Span.EqualTo("bytes(\"")
            from chars in Character.ExceptIn('"', '\\').Many()
            from close in Span.EqualTo("\")")
            select Convert.FromBase64String(new string(chars));

        public static TextParser<DateTime> DateTime { get; } =
            from open in Span.EqualTo("datetime(\"")
            from chars in Character.ExceptIn('"', '\\').Many()
            from close in Span.EqualTo("\")")
            select System.DateTime.Parse(new string(chars));

        public static TextParser<string> String { get; } =
            from open in Span.EqualTo("string(\"")
            from chars in Character.ExceptIn('"', '\\')
                .Or(Character.EqualTo('\\')
                    .IgnoreThen(
                        Character.EqualTo('\\')
                        .Or(Character.EqualTo('"'))
                        .Or(Character.EqualTo('/'))
                        .Or(Character.EqualTo('b').Value('\b'))
                        .Or(Character.EqualTo('f').Value('\f'))
                        .Or(Character.EqualTo('n').Value('\n'))
                        .Or(Character.EqualTo('r').Value('\r'))
                        .Or(Character.EqualTo('t').Value('\t'))
                        .Or(Character.EqualTo('u').IgnoreThen(
                                Span.MatchedBy(Character.HexDigit.Repeat(4))
                                    .Apply(Numerics.HexDigitsUInt32)
                                    .Select(cc => (char)cc)))
                        .Named("escape sequence")))
                .Many()
            from close in Span.EqualTo("\")")
            select new string(chars);

        public static TextParser<int> Integer { get; } =
            from open in Span.EqualTo("int(")
            from chars in Character.In(new[] { '-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }).Many()
            from close in Span.EqualTo(")")
            select Convert.ToInt32(new string(chars));

        public static TextParser<uint> UInteger { get; } =
            from open in Span.EqualTo("uint(")
            from chars in Character.Numeric.Many()
            from close in Span.EqualTo(")")
            select Convert.ToUInt32(new string(chars));

        public static TextParser<short> Short { get; } =
            from open in Span.EqualTo("short(")
            from chars in Character.In(new[] { '-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }).Many()
            from close in Span.EqualTo(")")
            select Convert.ToInt16(new string(chars));

        public static TextParser<ushort> UShort { get; } =
            from open in Span.EqualTo("ushort(")
            from chars in Character.Numeric.Many()
            from close in Span.EqualTo(")")
            select Convert.ToUInt16(new string(chars));

        public static TextParser<long> Long { get; } =
            from open in Span.EqualTo("long(")
            from chars in Character.In(new[] { '-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }).Many()
            from close in Span.EqualTo(")")
            select Convert.ToInt64(new string(chars));

        public static TextParser<ulong> ULong { get; } =
            from open in Span.EqualTo("ulong(")
            from chars in Character.Numeric.Many()
            from close in Span.EqualTo(")")
            select Convert.ToUInt64(new string(chars));

        public static TextParser<bool> Boolean { get; } =
            from open in Span.EqualTo("bool(")
            from chars in Character.ExceptIn(new[] { '(', ')' }).Many()
            from close in Span.EqualTo(")")
            select chars[0] == 't' ? true : false;

        public static TextParser<object> Null { get; } =
            from open in Span.EqualTo("null()")
            select (object)null;

        public static TextParser<char> Char { get; } =
            from open in Span.EqualTo("char(\"")
            from chars in Character.ExceptIn('"', '\\')
                .Or(Character.EqualTo('\\')
                    .IgnoreThen(
                        Character.EqualTo('\\')
                        .Or(Character.EqualTo('"'))
                        .Or(Character.EqualTo('/'))
                        .Or(Character.EqualTo('b').Value('\b'))
                        .Or(Character.EqualTo('f').Value('\f'))
                        .Or(Character.EqualTo('n').Value('\n'))
                        .Or(Character.EqualTo('r').Value('\r'))
                        .Or(Character.EqualTo('t').Value('\t'))
                        .Or(Character.EqualTo('u').IgnoreThen(
                                Span.MatchedBy(Character.HexDigit.Repeat(4))
                                    .Apply(Numerics.HexDigitsUInt32)
                                    .Select(cc => (char)cc)))
                        .Named("escape sequence")))
                .Many()
            from close in Span.EqualTo("\")")
            select Convert.ToChar(chars[0]);

        public static TextParser<byte> Byte { get; } =
            from open in Span.EqualTo("byte(")
            from chars in Character.In(new[] { '-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }).Many()
            from close in Span.EqualTo(")")
            select byte.Parse(new string(chars));

        public static TextParser<sbyte> SByte { get; } =
            from open in Span.EqualTo("sbyte(")
            from chars in Character.In(new[] { '-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }).Many()
            from close in Span.EqualTo(")")
            select sbyte.Parse(new string(chars));

        // The number parser here works with some pretty ugly `double`-based
        // maths; it's tricky enough that in a real-world parser, it would
        // be worth considering some imperative code to do this (see the way
        // that `Numerics.NaturalInt32` and similar parsers work, for examples).
        public static TextParser<double> Number { get; } =
            from sign in Character.EqualTo('-').Value(-1.0).OptionalOrDefault(1.0)
            from whole in Numerics.Natural.Select(n => double.Parse(n.ToStringValue()))
            from frac in Character.EqualTo('.')
                .IgnoreThen(Numerics.Natural)
                .Select(n => double.Parse(n.ToStringValue()) * Math.Pow(10, -n.Length))
                .OptionalOrDefault()
            from exp in Character.EqualToIgnoreCase('e')
                .IgnoreThen(Character.EqualTo('+').Value(1.0)
                    .Or(Character.EqualTo('-').Value(-1.0))
                    .OptionalOrDefault(1.0))
                .Then(expsign => Numerics.Natural.Select(n => double.Parse(n.ToStringValue()) * expsign))
                .OptionalOrDefault()
            select (whole + frac) * sign * Math.Pow(10, exp);

        public static TextParser<float> Float { get; } =
            from open in Span.EqualTo("float(")
            from sign in Character.EqualTo('-').Value(-1.0).OptionalOrDefault(1.0)
            from whole in Numerics.Natural.Select(n => double.Parse(n.ToStringValue()))
            from frac in Character.EqualTo('.')
                .IgnoreThen(Numerics.Natural)
                .Select(n => double.Parse(n.ToStringValue()) * Math.Pow(10, -n.Length))
                .OptionalOrDefault()
            from exp in Character.EqualToIgnoreCase('e')
                .IgnoreThen(Character.EqualTo('+').Value(1.0)
                    .Or(Character.EqualTo('-').Value(-1.0))
                    .OptionalOrDefault(1.0))
                .Then(expsign => Numerics.Natural.Select(n => double.Parse(n.ToStringValue()) * expsign))
                .OptionalOrDefault()
            from close in Span.EqualTo(")")
            select Convert.ToSingle((whole + frac) * sign * Math.Pow(10, exp));

        public static TextParser<double> Double { get; } =
            from open in Span.EqualTo("double(")
            from sign in Character.EqualTo('-').Value(-1.0).OptionalOrDefault(1.0)
            from whole in Numerics.Natural.Select(n => double.Parse(n.ToStringValue()))
            from frac in Character.EqualTo('.')
                .IgnoreThen(Numerics.Natural)
                .Select(n => double.Parse(n.ToStringValue()) * Math.Pow(10, -n.Length))
                .OptionalOrDefault()
            from exp in Character.EqualToIgnoreCase('e')
                .IgnoreThen(Character.EqualTo('+').Value(1.0)
                    .Or(Character.EqualTo('-').Value(-1.0))
                    .OptionalOrDefault(1.0))
                .Then(expsign => Numerics.Natural.Select(n => double.Parse(n.ToStringValue()) * expsign))
                .OptionalOrDefault()
            from close in Span.EqualTo(")")
            select (whole + frac) * sign * Math.Pow(10, exp);
    }
}
