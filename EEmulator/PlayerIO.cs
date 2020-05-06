using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Nancy;
using ProtoBuf;

namespace EEmulator
{
    public static class PlayerIO
    {
        public static byte[] PlayerTokenHeader(string token, bool checkHeader = true)
        {
            var output = new List<byte>();
            var length = BitConverter.GetBytes((ushort)token.Length).ToList();

            length.Reverse();

            output.Add((byte)(checkHeader ? 1 : 0));
            output.AddRange(length);
            output.AddRange(Encoding.UTF8.GetBytes(token));
            output.Add(1);

            return output.ToArray();
        }

        public static Response CreateResponse(string token, bool check, object body)
        {
            var response = new Response
            {
                Contents = s =>
                {
                    if (!string.IsNullOrEmpty(token))
                        s.WriteBytes(PlayerTokenHeader(token, check));

                    Serializer.Serialize(s, body);
                }
            };

            return response;
        }
    }

    internal static class StreamExtensions
    {
        internal static void WriteBytes(this Stream stream, byte[] bytes) => stream.Write(bytes, 0, bytes.Length);
    }
}
