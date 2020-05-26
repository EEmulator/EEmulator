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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Tson
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
        public static Dictionary<string, object> DeserializeObject(string input)
        {
            if (!TsonParser.TryParse(input, out var value, out var error, out var position))
                throw new TsonException("Unable to deserialize object. " + error + " at line " + position.Line + ", column: " + position.Column);

            return (Dictionary<string, object>)value;
        }
    }

    [Serializable]
    public class TsonException : Exception
    {
        internal TsonException() { }
        internal TsonException(string message) : base(message) { }
        internal TsonException(string message, Exception inner) : base(message, inner) { }
    }

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
                (value is byte[]) ? this.EscapeString(Convert.ToBase64String((byte[])value)) :
                (value is bool) ? ((bool)value ? "true" : "false") :
                (value is char) ? this.EscapeString("" + value) :
                (value is byte) ? (byte)value :
                (value is sbyte) ? (sbyte)value :
                (value is double) ? Math.Round((double)value, 10) :
                (value is DateTime) ? this.EscapeString(((DateTime)value).ToString("o")) : value).Append(")").ToString())
            :
            this.CheckTypeIsArray(value.GetType()) ? this.SerializeArray(value as IEnumerable<object>) :
            value.GetType().IsEnum ? string.Format("string({0})", this.EscapeString((string)value)) :
            value.GetType().IsValueType || value.GetType().IsClass ? this.SerializeObject(value) : null;

        private string SerializeObject(object input)
        {
            var builder = new StringBuilder().Append("{");

            if (input is IDictionary<string, object> || input is IDictionary<object, object>)
            {
                builder.Append(this.SerializeDictionary((IDictionary<string, object>)input));
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

        private string SerializeDictionary(IDictionary<string, object> input)
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

        private string SerializeArray(IEnumerable<object> collection) => "[" + collection.Cast<object>().Aggregate(new StringBuilder(),
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
