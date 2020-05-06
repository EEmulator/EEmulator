////////////////////////////////////////////////////////////////////////////////////
//    MIT License
//
//    Copyright (c) 2019 Atilla Lonny (https://github.com/atillabyte/tson)
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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.ComponentModel;

namespace Tson.NET
{
    public enum Formatting
    {
        None,
        Indented
    }

    public static class TsonConvert
    {
        /// <summary>
        /// Serializes the specified object to a TSON string using formatting.
        /// </summary>
        /// <param name="value"> The object to serialize. </param>
        /// <param name="formatting"> Indicates how the output should be formatted. </param>
        /// <param name="includePrivate"> Indicates whether to include private properties and members in serialization. </param>
        /// <returns>
        /// A TSON string representation of the object.
        /// </returns>
        public static string SerializeObject(object input, Formatting format = Formatting.None, bool includePrivate = false)
        {
            var writer = new TsonWriter(includePrivate);

            switch (format)
            {
                case Formatting.None:
                    return writer.Serialize(input);
                case Formatting.Indented:
                    return TsonFormat.Format(writer.Serialize(input));
                default:
                    throw new TsonException("The formatting specified is invalid.");
            }
        }

        /// <summary>
        /// Deserializes the TSON string into a dictionary.
        /// </summary>
        /// <param name="input"> The TSON to deserialize. </param>
        /// <returns> A dictionary containing the deserialized items. </returns>
        public static Dictionary<string, object> DeserializeObject(string input) => new TsonReader().Parse(input) as Dictionary<string, object>;
    }

    [Serializable]
    public class TsonException : Exception
    {
        internal TsonException() { }
        internal TsonException(string message) : base(message) { }
        internal TsonException(string message, Exception inner) : base(message, inner) { }
    }
}

/// <summary>
/// TsonReader / TsonWriter / TsonFormat
/// </summary>
namespace Tson.NET
{
    internal static class TsonValueMap
    {
        public static Dictionary<Type, string> Dictionary = new Dictionary<Type, string>()
        {
            { typeof(string), "string" },   { typeof(byte[]), "bytes"  },   { typeof(char), "char"         },
            { typeof(bool),   "bool"   },   { typeof(int),    "int"    },   { typeof(byte), "byte"         },
            { typeof(sbyte),  "sbyte"  },   { typeof(short),  "short"  },   { typeof(ushort), "ushort"     },
            { typeof(uint),   "uint"   },   { typeof(long),   "long"   },   { typeof(ulong), "ulong"       },
            { typeof(float),  "float"  },   { typeof(double), "double" },   { typeof(DateTime), "datetime" }
        };

        public static string TypeToName(Type type) => Dictionary[type];
        public static Type NameToType(string name) => Dictionary.First(n => n.Value == name).Key;
    }

    internal class TsonReader
    {
        private enum Token
        {
            None,
            OpenBrace,
            CloseBrace,
            OpenBracket,
            CloseBracket,
            Colon,
            Comma,
            String,
            ValueExpression
        }

        private StringReader StringReader;

        public object Parse(string input)
        {
            this.StringReader = new StringReader(Regex.Replace(input, @"(""(?:[^""\\]|\\.)*"")|\s+", "$1")); // remove whitespace

            if (this.StringReader.Peek() == '{')
                return this.DecodeObject();

            if (this.StringReader.Peek() == '[')
                return this.DecodeArray();

            return null;
        }

        private Dictionary<string, object> DecodeObject()
        {
            this.StringReader.Read(); // skip opening brace
            var dictionary = new Dictionary<string, object>();

            while (true)
            {
                switch (this.NextToken())
                {
                    case Token.None:
                        return null;

                    case Token.Comma:
                        continue;

                    case Token.CloseBrace:
                        return dictionary;

                    default:
                        var key = this.DecodeString();

                        if (string.IsNullOrEmpty(key))
                            return null;

                        if (this.NextToken() != Token.Colon)
                            return null;

                        this.StringReader.Read();

                        var value = this.DecodeFromToken(this.NextToken());

                        dictionary.Add(key, value);
                        break;
                }
            }
        }

        private List<object> DecodeArray()
        {
            var list = new List<object>();
            this.StringReader.Read(); // skip opening bracket

            var parsing = true;
            while (parsing)
            {
                switch (this.NextToken())
                {
                    case Token.None:
                        return null;

                    case Token.Comma:
                        continue;

                    case Token.CloseBracket:
                        parsing = false;
                        break;

                    default:
                        list.Add(this.DecodeFromToken(this.NextToken()));
                        break;
                }
            }

            return list;
        }

        private object DecodeFromToken(Token token)
        {
            switch (token)
            {
                case Token.String:
                    return this.DecodeString();

                case Token.OpenBrace:
                    return this.DecodeObject();

                case Token.OpenBracket:
                    return this.DecodeArray();

                case Token.ValueExpression:
                    return this.DecodeValueExpression();
            }

            return null;
        }

        private object DecodeValueExpression()
        {
            var keyword = new StringBuilder();

            var inside_string = false;
            while ("{}[],:".IndexOf((char)this.StringReader.Peek()) == -1 || inside_string)
            {
                var n = (char)this.StringReader.Read();
                keyword.Append(n);

                if (n == '"')
                    inside_string = !inside_string;

                if (this.StringReader.Peek() == -1)
                    break;
            }

            var match = Regex.Match(keyword.ToString(), @"(\w+)\(([^()]+|[^(]+\([^)]*\)[^()]*)\)");

            var type = match.Groups[1].Value;
            var value = match.Groups[2].Value;

            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(value))
                return null;

            switch (type)
            {
                case "string":
                    return value.Remove(value.Length - 1, 1).Remove(0, 1);

                case "char":
                    if (string.IsNullOrEmpty(value) || value.Length <= 2 || !(value[0] == '"' && (value[value.Length - 1] == '"')))
                        return null;

                    return value.Remove(value.Length - 1, 1).Remove(0, 1).First();

                case "bytes":
                    if (string.IsNullOrEmpty(value) || value.Length <= 2 || !(value[0] == '"' && (value[value.Length - 1] == '"')))
                        return null;

                    return Convert.FromBase64String(value.Remove(value.Length - 1, 1).Remove(0, 1));

                case "datetime":
                    if (string.IsNullOrEmpty(value) || value.Length <= 2 || !(value[0] == '"' && (value[value.Length - 1] == '"')))
                        return null;

                    return DateTime.Parse(value.Remove(value.Length - 1, 1).Remove(0, 1), null, DateTimeStyles.RoundtripKind);

                case "null":
                    return null;

                default:
                    if (TsonValueMap.Dictionary.ContainsValue(type))
                        return this.ConvertFromType(value, TsonValueMap.NameToType(type));
                    break;
            }

            return null;
        }

        private object ConvertFromType(string input, Type type)
        {
            try
            {
                var converter = TypeDescriptor.GetConverter(type);
                if (converter != null)
                    return converter.ConvertFromString(input);

                return null;
            }
            catch (NotSupportedException)
            {
                return null;
            }
        }

        private string DecodeString()
        {
            var stringBuilder = new StringBuilder();
            this.StringReader.Read(); // skip opening quote

            var parsing = true;
            while (parsing)
            {
                if (this.StringReader.Peek() == -1)
                {
                    parsing = false;
                    break;
                }

                var c = (char)this.StringReader.Read();
                switch (c)
                {
                    case '"':
                        parsing = false;
                        break;

                    case '\\':
                        if (this.StringReader.Peek() == -1)
                        {
                            parsing = false;
                            break;
                        }

                        c = (char)this.StringReader.Read();

                        switch (c)
                        {
                            case '"':
                            case '\\':
                            case '/':
                                stringBuilder.Append(c);
                                break;

                            case 'b':
                                stringBuilder.Append('\b');
                                break;

                            case 'f':
                                stringBuilder.Append('\f');
                                break;

                            case 'n':
                                stringBuilder.Append('\n');
                                break;

                            case 'r':
                                stringBuilder.Append('\r');
                                break;

                            case 't':
                                stringBuilder.Append('\t');
                                break;

                            case 'u':
                                var hex = new StringBuilder();

                                for (var i = 0; i < 4; i++)
                                {
                                    hex.Append(this.StringReader.Read());
                                }

                                stringBuilder.Append((char)Convert.ToInt32(hex.ToString(), 16));
                                break;
                        }

                        break;

                    default:
                        stringBuilder.Append(c);
                        break;
                }
            }

            return stringBuilder.ToString();
        }

        private Token NextToken()
        {
            if (this.StringReader.Peek() == -1)
                return Token.None;

            switch (this.StringReader.Peek())
            {
                case '{':
                    return Token.OpenBrace;

                case '}':
                    this.StringReader.Read();
                    return Token.CloseBrace;

                case '[':
                    return Token.OpenBracket;

                case ']':
                    this.StringReader.Read();
                    return Token.CloseBracket;

                case ',':
                    this.StringReader.Read();
                    return Token.Comma;

                case '"':
                    return Token.String;

                case ':':
                    return Token.Colon;
            }

            return Token.ValueExpression;
        }
    }

    internal class TsonWriter
    {
        private BindingFlags MemberFlags { get; }

        internal TsonWriter(bool includePrivateMembers) =>
            this.MemberFlags = BindingFlags.Instance | BindingFlags.Public | (includePrivateMembers ? BindingFlags.NonPublic : 0);

        internal string Serialize(object input) => this.SerializeValue(input);

        private string SerializeValue(object value) =>
            (TsonValueMap.Dictionary.ContainsKey(value.GetType())) ?
            (new StringBuilder().Append(TsonValueMap.TypeToName(value.GetType())).Append("(").Append(
                (value is string) ? this.EscapeString((string)value) :
                (value is byte[]) ? Convert.ToBase64String((byte[])value) :
                (value is bool) ? ((bool)value ? "true" : "false") :
                (value is char) ? this.EscapeString("" + value) :
                (value is DateTime) ? this.EscapeString(((DateTime)value).ToString("o")) : value).Append(")").ToString())
            :
            this.CheckTypeIsArray(value.GetType()) ? this.SerializeArray(value as IEnumerable) :
            value.GetType().IsEnum ? string.Format("string({0})", this.EscapeString((string)value)) :
            value.GetType().IsValueType || value.GetType().IsClass ? this.SerializeObject(value) : null;

        private string SerializeObject(object input)
        {
            var builder = new StringBuilder().Append("{");

            if (input is IDictionary<string, object> || input is IDictionary)
            {
                builder.Append(this.SerializeDictionary((IDictionary)input));
            }
            else
            {
                var memberValues = input.GetType().GetMembers(this.MemberFlags).ToList().Select(member =>
                    (member.MemberType == MemberTypes.Field && !(member as FieldInfo).IsStatic) ? (member.Name, (member as FieldInfo).GetValue(input)) :
                    (member.MemberType == MemberTypes.Property ? (member.Name, (member as PropertyInfo).GetValue(input)) : (null, null)));

                var first_value = true;
                foreach (var item in memberValues)
                {
                    var (name, value) = item;

                    if (value != null)
                    {
                        if (!first_value)
                            builder.Append(",");

                        builder.Append(this.EscapeString(name));
                        builder.Append(":");
                        builder.Append(this.SerializeValue(value));
                        first_value = false;
                    }
                }
            }

            return builder.Append("}").ToString();
        }

        private string SerializeDictionary(IDictionary input)
        {
            var builder = new StringBuilder();

            var first_value = true;
            foreach (var key in input.Keys)
            {
                if (!first_value)
                    builder.Append(',');

                builder.Append(this.EscapeString(key.ToString()));
                builder.Append(':');
                builder.Append(this.SerializeValue(input[key]));

                first_value = false;
            }

            return builder.ToString();
        }

        private string EscapeString(string input)
        {
            var builder = new StringBuilder().Append('\"');

            foreach (var c in input.ToCharArray())
            {
                switch (c)
                {
                    case '"': builder.Append("\\\""); break;
                    case '\\': builder.Append("\\\\"); break;
                    case '\b': builder.Append("\\b"); break;
                    case '\f': builder.Append("\\f"); break;
                    case '\n': builder.Append("\\n"); break;
                    case '\r': builder.Append("\\r"); break;
                    case '\t': builder.Append("\\t"); break;
                    default:
                        var codepoint = Convert.ToInt32(c);

                        builder.Append((codepoint >= 32) && (codepoint <= 126) ? "" + c : "\\u" + codepoint.ToString("x4"));
                        break;
                }
            }

            return builder.Append('\"').ToString();
        }

        private string SerializeArray(IEnumerable collection) => "[" + collection.Cast<object>().Aggregate(new StringBuilder(),
             (sb, v) => sb.Append(this.SerializeValue(v)).Append(","), sb => { if (0 < sb.Length) sb.Length--; return sb.ToString(); }) + "]";

        private bool CheckTypeIsArray(Type type) => type.IsArray || (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>))) || typeof(IEnumerable<object>).IsAssignableFrom(type);
    }

    internal static class TsonFormat
    {
        internal const string INDENT_STRING = "    ";

        internal static string Format(string input)
        {
            var indent = 0;
            var quoted = false;
            var sb = new StringBuilder();

            for (var i = 0; i < input.Length; i++)
            {
                var ch = input[i];
                switch (ch)
                {
                    case '{':
                    case '[':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, ++indent).ForEach(item => sb.Append(INDENT_STRING));
                        }
                        break;
                    case '}':
                    case ']':
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, --indent).ForEach(item => sb.Append(INDENT_STRING));
                        }
                        sb.Append(ch);
                        break;
                    case '"':
                        sb.Append(ch);
                        var escaped = false;
                        var index = i;
                        while (index > 0 && input[--index] == '\\')
                            escaped = !escaped;
                        if (!escaped)
                            quoted = !quoted;
                        break;
                    case ',':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, indent).ForEach(item => sb.Append(INDENT_STRING));
                        }
                        break;
                    case ':':
                        sb.Append(ch);
                        if (!quoted)
                            sb.Append(" ");
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }
            return sb.ToString();
        }

        internal static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
        {
            foreach (var i in ie)
                action(i);
        }
    }
}