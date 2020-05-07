using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace EverybodyEdits.Auth
{
    public class IndexAndSuccess
    {
        public int index = 0;
        public bool success = false;
    }

    /// <summary>
    ///     This class encodes and decodes JSON strings.
    ///     Spec. details, see http://www.json.org/
    ///     JSON uses Arrays and Objects. These correspond here to the datatypes ArrayList and Hashtable.
    ///     All numbers are parsed to doubles.
    /// </summary>
    public class JSON
    {
        public const int TOKEN_NONE = 0;
        public const int TOKEN_CURLY_OPEN = 1;
        public const int TOKEN_CURLY_CLOSE = 2;
        public const int TOKEN_SQUARED_OPEN = 3;
        public const int TOKEN_SQUARED_CLOSE = 4;
        public const int TOKEN_COLON = 5;
        public const int TOKEN_COMMA = 6;
        public const int TOKEN_STRING = 7;
        public const int TOKEN_NUMBER = 8;
        public const int TOKEN_TRUE = 9;
        public const int TOKEN_FALSE = 10;
        public const int TOKEN_NULL = 11;

        private const int BUILDER_CAPACITY = 2000;

        /// <summary>
        ///     Parses the string json into a value
        /// </summary>
        /// <param name="json">A JSON string.</param>
        /// <returns>An ArrayList, a Hashtable, a double, a string, null, true, or false</returns>
        public object JsonDecode(string json)
        {
            var ias = new IndexAndSuccess();
            ias.success = true;

            return this.JsonDecode(json, ias);
        }

        /// <summary>
        ///     Parses the string json into a value; and fills 'success' with the successfullness of the parse.
        /// </summary>
        /// <param name="json">A JSON string.</param>
        /// <param name="success">Successful parse?</param>
        /// <returns>An ArrayList, a Hashtable, a double, a string, null, true, or false</returns>
        public object JsonDecode(string json, IndexAndSuccess ias)
        {
            ias.success = true;
            if (json != null)
            {
                var charArray = json.ToCharArray();
                ias.index = 0;
                var value = this.ParseValue(charArray, ias);
                return value;
            }
            return null;
        }

        /// <summary>
        ///     Converts a Hashtable / ArrayList object into a JSON string
        /// </summary>
        /// <param name="json">A Hashtable / ArrayList</param>
        /// <returns>A JSON encoded string, or null if object 'json' is not serializable</returns>
        public string JsonEncode(object json)
        {
            var builder = new StringBuilder(BUILDER_CAPACITY);
            var success = this.SerializeValue(json, builder);
            return (success ? builder.ToString() : null);
        }

        protected Hashtable ParseObject(char[] json, IndexAndSuccess ias)
        {
            var table = new Hashtable();
            int token;

            // {
            this.NextToken(json, ias);

            var done = false;
            while (!done)
            {
                token = this.LookAhead(json, ias.index);
                if (token == TOKEN_NONE)
                {
                    ias.success = false;
                    return null;
                }
                if (token == TOKEN_COMMA)
                {
                    this.NextToken(json, ias);
                }
                else if (token == TOKEN_CURLY_CLOSE)
                {
                    this.NextToken(json, ias);
                    return table;
                }
                else
                {
                    // name
                    var name = this.ParseString(json, ias);
                    if (!ias.success)
                    {
                        ias.success = false;
                        return null;
                    }

                    // :
                    token = this.NextToken(json, ias);
                    if (token != TOKEN_COLON)
                    {
                        ias.success = false;
                        return null;
                    }

                    // value
                    var value = this.ParseValue(json, ias);
                    if (!ias.success)
                    {
                        ias.success = false;
                        return null;
                    }

                    table[name] = value;
                }
            }

            return table;
        }

        protected ArrayList ParseArray(char[] json, IndexAndSuccess ias)
        {
            var array = new ArrayList();

            // [
            this.NextToken(json, ias);

            var done = false;
            while (!done)
            {
                var token = this.LookAhead(json, ias.index);
                if (token == TOKEN_NONE)
                {
                    ias.success = false;
                    return null;
                }
                if (token == TOKEN_COMMA)
                {
                    this.NextToken(json, ias);
                }
                else if (token == TOKEN_SQUARED_CLOSE)
                {
                    this.NextToken(json, ias);
                    break;
                }
                else
                {
                    var value = this.ParseValue(json, ias);
                    if (!ias.success)
                    {
                        return null;
                    }

                    array.Add(value);
                }
            }

            return array;
        }

        protected object ParseValue(char[] json, IndexAndSuccess ias)
        {
            switch (this.LookAhead(json, ias.index))
            {
                case TOKEN_STRING:
                    return this.ParseString(json, ias);
                case TOKEN_NUMBER:
                    return this.ParseNumber(json, ias);
                case TOKEN_CURLY_OPEN:
                    return this.ParseObject(json, ias);
                case TOKEN_SQUARED_OPEN:
                    return this.ParseArray(json, ias);
                case TOKEN_TRUE:
                    this.NextToken(json, ias);
                    return true;
                case TOKEN_FALSE:
                    this.NextToken(json, ias);
                    return false;
                case TOKEN_NULL:
                    this.NextToken(json, ias);
                    return null;
                case TOKEN_NONE:
                    break;
            }

            ias.success = false;
            return null;
        }

        protected string ParseString(char[] json, IndexAndSuccess ias)
        {
            var s = new StringBuilder(BUILDER_CAPACITY);
            char c;

            this.EatWhitespace(json, ias);

            // "
            c = json[ias.index++];

            var complete = false;
            while (!complete)
            {
                if (ias.index == json.Length)
                {
                    break;
                }

                c = json[ias.index++];
                if (c == '"')
                {
                    complete = true;
                    break;
                }
                if (c == '\\')
                {
                    if (ias.index == json.Length)
                    {
                        break;
                    }
                    c = json[ias.index++];
                    if (c == '"')
                    {
                        s.Append('"');
                    }
                    else if (c == '\\')
                    {
                        s.Append('\\');
                    }
                    else if (c == '/')
                    {
                        s.Append('/');
                    }
                    else if (c == 'b')
                    {
                        s.Append('\b');
                    }
                    else if (c == 'f')
                    {
                        s.Append('\f');
                    }
                    else if (c == 'n')
                    {
                        s.Append('\n');
                    }
                    else if (c == 'r')
                    {
                        s.Append('\r');
                    }
                    else if (c == 't')
                    {
                        s.Append('\t');
                    }
                    else if (c == 'u')
                    {
                        var remainingLength = json.Length - ias.index;
                        if (remainingLength >= 4)
                        {
                            // parse the 32 bit hex into an integer codepoint
                            uint codePoint;
                            if (
                                !(ias.success =
                                    UInt32.TryParse(new string(json, ias.index, 4), NumberStyles.HexNumber,
                                        CultureInfo.InvariantCulture, out codePoint)))
                            {
                                return "";
                            }
                            // convert the integer codepoint to a unicode char and add to string
                            s.Append(Char.ConvertFromUtf32((int)codePoint));
                            // skip 4 chars
                            ias.index += 4;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    s.Append(c);
                }
            }

            if (!complete)
            {
                ias.success = false;
                return null;
            }

            return s.ToString();
        }

        protected double ParseNumber(char[] json, IndexAndSuccess ias)
        {
            this.EatWhitespace(json, ias);

            var lastIndex = this.GetLastIndexOfNumber(json, ias.index);
            var charLength = (lastIndex - ias.index) + 1;

            double number;
            ias.success = Double.TryParse(new string(json, ias.index, charLength), NumberStyles.Any,
                CultureInfo.InvariantCulture, out number);

            ias.index = lastIndex + 1;
            return number;
        }

        protected int GetLastIndexOfNumber(char[] json, int index)
        {
            int lastIndex;

            for (lastIndex = index; lastIndex < json.Length; lastIndex++)
            {
                if ("0123456789+-.eE".IndexOf(json[lastIndex]) == -1)
                {
                    break;
                }
            }
            return lastIndex - 1;
        }

        protected void EatWhitespace(char[] json, IndexAndSuccess ias)
        {
            for (; ias.index < json.Length; ias.index++)
            {
                if (" \t\n\r".IndexOf(json[ias.index]) == -1)
                {
                    break;
                }
            }
        }

        protected int LookAhead(char[] json, int index)
        {
            var ias = new IndexAndSuccess();
            ias.index = index;
            return this.NextToken(json, ias);
        }

        protected int NextToken(char[] json, IndexAndSuccess ias)
        {
            this.EatWhitespace(json, ias);

            if (ias.index == json.Length)
            {
                return TOKEN_NONE;
            }

            var c = json[ias.index];
            ias.index++;
            switch (c)
            {
                case '{':
                    return TOKEN_CURLY_OPEN;
                case '}':
                    return TOKEN_CURLY_CLOSE;
                case '[':
                    return TOKEN_SQUARED_OPEN;
                case ']':
                    return TOKEN_SQUARED_CLOSE;
                case ',':
                    return TOKEN_COMMA;
                case '"':
                    return TOKEN_STRING;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '-':
                    return TOKEN_NUMBER;
                case ':':
                    return TOKEN_COLON;
            }
            ias.index--;

            var remainingLength = json.Length - ias.index;

            // false
            if (remainingLength >= 5)
            {
                if (json[ias.index] == 'f' &&
                    json[ias.index + 1] == 'a' &&
                    json[ias.index + 2] == 'l' &&
                    json[ias.index + 3] == 's' &&
                    json[ias.index + 4] == 'e')
                {
                    ias.index += 5;
                    return TOKEN_FALSE;
                }
            }

            // true
            if (remainingLength >= 4)
            {
                if (json[ias.index] == 't' &&
                    json[ias.index + 1] == 'r' &&
                    json[ias.index + 2] == 'u' &&
                    json[ias.index + 3] == 'e')
                {
                    ias.index += 4;
                    return TOKEN_TRUE;
                }
            }

            // null
            if (remainingLength >= 4)
            {
                if (json[ias.index] == 'n' &&
                    json[ias.index + 1] == 'u' &&
                    json[ias.index + 2] == 'l' &&
                    json[ias.index + 3] == 'l')
                {
                    ias.index += 4;
                    return TOKEN_NULL;
                }
            }

            return TOKEN_NONE;
        }

        protected bool SerializeValue(object value, StringBuilder builder)
        {
            var success = true;

            if (value is string)
            {
                success = this.SerializeString((string)value, builder);
            }
            else if (value is Hashtable)
            {
                success = this.SerializeObject((Hashtable)value, builder);
            }
            else if (value is ArrayList)
            {
                success = this.SerializeArray((ArrayList)value, builder);
            }
            else if (this.IsNumeric(value))
            {
                success = this.SerializeNumber(Convert.ToDouble(value), builder);
            }
            else if ((value is Boolean) && (Boolean)value)
            {
                builder.Append("true");
            }
            else if ((value is Boolean) && ((Boolean)value == false))
            {
                builder.Append("false");
            }
            else if (value == null)
            {
                builder.Append("null");
            }
            else
            {
                success = false;
            }
            return success;
        }

        protected bool SerializeObject(Hashtable anObject, StringBuilder builder)
        {
            builder.Append("{");

            var e = anObject.GetEnumerator();
            var first = true;
            while (e.MoveNext())
            {
                var key = e.Key.ToString();
                var value = e.Value;

                if (!first)
                {
                    builder.Append(", ");
                }

                this.SerializeString(key, builder);
                builder.Append(":");
                if (!this.SerializeValue(value, builder))
                {
                    return false;
                }

                first = false;
            }

            builder.Append("}");
            return true;
        }

        protected bool SerializeArray(ArrayList anArray, StringBuilder builder)
        {
            builder.Append("[");

            var first = true;
            for (var i = 0; i < anArray.Count; i++)
            {
                var value = anArray[i];

                if (!first)
                {
                    builder.Append(", ");
                }

                if (!this.SerializeValue(value, builder))
                {
                    return false;
                }

                first = false;
            }

            builder.Append("]");
            return true;
        }

        protected bool SerializeString(string aString, StringBuilder builder)
        {
            builder.Append("\"");

            var charArray = aString.ToCharArray();
            for (var i = 0; i < charArray.Length; i++)
            {
                var c = charArray[i];
                if (c == '"')
                {
                    builder.Append("\\\"");
                }
                else if (c == '\\')
                {
                    builder.Append("\\\\");
                }
                else if (c == '\b')
                {
                    builder.Append("\\b");
                }
                else if (c == '\f')
                {
                    builder.Append("\\f");
                }
                else if (c == '\n')
                {
                    builder.Append("\\n");
                }
                else if (c == '\r')
                {
                    builder.Append("\\r");
                }
                else if (c == '\t')
                {
                    builder.Append("\\t");
                }
                else
                {
                    var codepoint = Convert.ToInt32(c);
                    if ((codepoint >= 32) && (codepoint <= 126))
                    {
                        builder.Append(c);
                    }
                    else
                    {
                        builder.Append("\\u" + Convert.ToString(codepoint, 16).PadLeft(4, '0'));
                    }
                }
            }

            builder.Append("\"");
            return true;
        }

        protected bool SerializeNumber(double number, StringBuilder builder)
        {
            builder.Append(Convert.ToString(number, CultureInfo.InvariantCulture));
            return true;
        }

        /// <summary>
        ///     Determines if a given object is numeric in any way
        ///     (can be integer, double, null, etc).
        ///     Thanks to mtighe for pointing out Double.TryParse to me.
        /// </summary>
        protected bool IsNumeric(object o)
        {
            double result;

            return (o == null) ? false : Double.TryParse(o.ToString(), out result);
        }
    }
}