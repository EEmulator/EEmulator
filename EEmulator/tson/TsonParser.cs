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

using System.Collections.Generic;
using System.Linq;
using Superpower;
using Superpower.Parsers;

namespace Tson
{
    public static class TsonParser
    {
        // For simplicity, we use `object` as the stand-in for every
        // possible TSON value type. There's quite a lot of casting:
        // unfortunately, for performance reasons, Superpower uses a
        // parser design that doesn't allow for variance, so you need
        // to create a parser that returns `object` here, even though
        // one that returns `string` should, in theory, be compatible.
        static TokenListParser<TsonToken, object> TsonPropertyString { get; } =
            Token.EqualTo(TsonToken.PropertyString)
                .Apply(TsonTextParsers.PropertyString)
                .Select(s => (object)s);

        static TokenListParser<TsonToken, object> TsonByteArray { get; } =
            Token.EqualTo(TsonToken.ByteArray)
                .Apply(TsonTextParsers.ByteArray)
                .Select(s => (object)s);

        static TokenListParser<TsonToken, object> TsonString { get; } =
            Token.EqualTo(TsonToken.String)
                .Apply(TsonTextParsers.String)
                .Select(s => (object)s);

        static TokenListParser<TsonToken, object> TsonInteger { get; } =
            Token.EqualTo(TsonToken.Integer)
                .Apply(TsonTextParsers.Integer)
                .Select(s => (object)s);

        static TokenListParser<TsonToken, object> TsonNumber { get; } =
            Token.EqualTo(TsonToken.Number)
                .Apply(TsonTextParsers.Number)
                .Select(n => (object)n);

        static TokenListParser<TsonToken, object> TsonUInteger { get; } =
            Token.EqualTo(TsonToken.UInteger)
                .Apply(TsonTextParsers.UInteger)
                .Select(n => (object)n);

        static TokenListParser<TsonToken, object> TsonBoolean { get; } =
            Token.EqualTo(TsonToken.Boolean)
                .Apply(TsonTextParsers.Boolean)
                .Select(n => (object)n);

        static TokenListParser<TsonToken, object> TsonCharacter { get; } =
            Token.EqualTo(TsonToken.Char)
                .Apply(TsonTextParsers.Char)
                .Select(n => (object)n);

        static TokenListParser<TsonToken, object> TsonByte { get; } =
            Token.EqualTo(TsonToken.Byte)
                .Apply(TsonTextParsers.Byte)
                .Select(n => (object)n);

        static TokenListParser<TsonToken, object> TsonSByte { get; } =
            Token.EqualTo(TsonToken.SByte)
                .Apply(TsonTextParsers.SByte)
                .Select(n => (object)n);

        static TokenListParser<TsonToken, object> TsonShort { get; } =
            Token.EqualTo(TsonToken.Short)
                .Apply(TsonTextParsers.Short)
                .Select(n => (object)n);

        static TokenListParser<TsonToken, object> TsonUShort { get; } =
            Token.EqualTo(TsonToken.UShort)
                .Apply(TsonTextParsers.UShort)
                .Select(n => (object)n);

        static TokenListParser<TsonToken, object> TsonLong { get; } =
            Token.EqualTo(TsonToken.Long)
                .Apply(TsonTextParsers.Long)
                .Select(n => (object)n);

        static TokenListParser<TsonToken, object> TsonULong { get; } =
            Token.EqualTo(TsonToken.ULong)
                .Apply(TsonTextParsers.ULong)
                .Select(n => (object)n);

        static TokenListParser<TsonToken, object> TsonFloat { get; } =
            Token.EqualTo(TsonToken.Float)
                .Apply(TsonTextParsers.Float)
                .Select(n => (object)n);

        static TokenListParser<TsonToken, object> TsonDouble { get; } =
            Token.EqualTo(TsonToken.Double)
                .Apply(TsonTextParsers.Double)
                .Select(n => (object)n);

        static TokenListParser<TsonToken, object> TsonDateTime { get; } =
            Token.EqualTo(TsonToken.DateTime)
                .Apply(TsonTextParsers.DateTime)
                .Select(n => (object)n);

        static TokenListParser<TsonToken, object> TsonNull { get; } =
            Token.EqualTo(TsonToken.Null)
                .Apply(TsonTextParsers.Null)
                .Select(n => (object)n);

        // The grammar is recursive - values can be objects, which contain
        // values, which can be objects... In order to reflect this circularity,
        // the parser below uses `Parse.Ref()` to refer lazily to the `TsonValue`
        // parser, which won't be constructed until after the runtime initializes
        // the `TsonObject` parser.
        static TokenListParser<TsonToken, object> TsonObject { get; } =
            from begin in Token.EqualTo(TsonToken.LBracket)
            from properties in TsonPropertyString
                .Named("property name")
                .Then(name => Token.EqualTo(TsonToken.Colon)
                    .IgnoreThen(Parse.Ref(() => TsonValue)
                    .Select(value => new KeyValuePair<string, object>((string)name, value))))
                .ManyDelimitedBy(Token.EqualTo(TsonToken.Comma),
                    end: Token.EqualTo(TsonToken.RBracket))
            select (object)properties.ToDictionary(k => k.Key, v => v.Value);

        // `ManyDelimitedBy()` is a convenience helper for parsing lists that contain
        // separators. Specifying an `end` delimiter improves error reporting by enabling
        // expectations like "expected (item) or (close delimiter)" when no content matches.
        static TokenListParser<TsonToken, object> TsonArray { get; } =
            from begin in Token.EqualTo(TsonToken.LSquareBracket)
            from values in Parse.Ref(() => TsonValue)
                .ManyDelimitedBy(Token.EqualTo(TsonToken.Comma),
                    end: Token.EqualTo(TsonToken.RSquareBracket))
            select (object)values;

        static TokenListParser<TsonToken, object> TsonTrue { get; } =
            Token.EqualToValue(TsonToken.Identifier, "true").Value((object)true);

        static TokenListParser<TsonToken, object> TsonFalse { get; } =
            Token.EqualToValue(TsonToken.Identifier, "false").Value((object)false);

        static TokenListParser<TsonToken, object> TsonLegacyNull { get; } =
            Token.EqualToValue(TsonToken.Identifier, "null").Value((object)null);

        static TokenListParser<TsonToken, object> TsonValue { get; } =
            TsonPropertyString
                .Or(TsonNumber)
                .Or(TsonObject)
                .Or(TsonArray)
                .Or(TsonTrue)
                .Or(TsonFalse)
                .Or(TsonLegacyNull)
                .Or(TsonString)
                .Or(TsonInteger)
                .Or(TsonDateTime)
                .Or(TsonByteArray)
                .Or(TsonUInteger)
                .Or(TsonBoolean)
                .Or(TsonCharacter)
                .Or(TsonByte)
                .Or(TsonSByte)
                .Or(TsonShort)
                .Or(TsonUShort)
                .Or(TsonLong)
                .Or(TsonULong)
                .Or(TsonFloat)
                .Or(TsonDouble)
                .Or(TsonNull)
                .Named("TSON value");

        static TokenListParser<TsonToken, object> TsonDocument { get; } = TsonValue.AtEnd();

        // `TryParse` is just a helper method. It's useful to write one of these, where
        // the tokenization and parsing phases remain distinct, because it's often very
        // handy to place a breakpoint between the two steps to check out what the
        // token list looks like.
        public static bool TryParse(string tson, out object value, out string error, out ErrorPosition errorPosition)
        {
            var tokens = TsonTokenizer.Instance.TryTokenize(tson);
            if (!tokens.HasValue)
            {
                value = null;
                error = tokens.ToString();
                errorPosition = new ErrorPosition(tokens.ErrorPosition.Absolute, tokens.ErrorPosition.Line, tokens.ErrorPosition.Column);
                return false;
            }

            var parsed = TsonDocument.TryParse(tokens.Value);
            if (!parsed.HasValue)
            {
                value = null;
                error = parsed.ToString();
                errorPosition = new ErrorPosition(parsed.ErrorPosition.Absolute, parsed.ErrorPosition.Line, parsed.ErrorPosition.Column);
                return false;
            }

            value = parsed.Value;
            error = null;
            errorPosition = new ErrorPosition(0, 0, 0);
            return true;
        }
    }

    public struct ErrorPosition
    {
        public ErrorPosition(int absolute, int line, int column)
        {
            this.Absolute = absolute;
            this.Line = line;
            this.Column = column;
        }

        public int Absolute { get; }
        public int Line { get; }
        public int Column { get; }
    }
}
