using System.Text;

namespace EverybodyEdits.Game
{
    internal class UrlEncoder
    {
        public static string UrlEncode(Encoding encoding, string input)
        {
            var bytes = encoding.GetBytes(input);

            var sb = new StringBuilder();
            for (var i = 0; i != bytes.Length; i++)
            {
                var chr = (char)bytes[i];
                if (isSafe(chr))
                {
                    sb.Append(chr);
                }
                else
                {
                    sb.Append("%" + bytes[i].ToString("X2"));
                }
            }
            return sb.ToString();
        }


        private static bool isSafe(char chr)
        {
            if (chr >= 97 && chr <= 122 || chr >= 65 && chr <= 90 || chr >= 48 && chr <= 57) return true;
            switch (chr)
            {
                case '!':
                case '(':
                case ')':
                case '*':
                case '-':
                case '.':
                case '_':
                    return true;
                default:
                    return false;
            }
        }
    }
}