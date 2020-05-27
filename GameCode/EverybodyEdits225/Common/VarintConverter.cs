using System.Collections.Generic;

namespace EverybodyEdits.Common
{
    public class VarintConverter
    {
        public static byte[] GetVarintBytes(IEnumerable<int> values)
        {
            var bytes = new List<byte>();
            foreach (var value in values)
            {
                bytes.AddRange(GetVarintBytes(value));
            }
            return bytes.ToArray();
        }

        public static List<int> ToInt32List(byte[] bytes)
        {
            var shift = 0;
            var result = 0U;

            var results = new List<int>();

            foreach (uint byteValue in bytes)
            {
                var tmp = byteValue & 0x7f;
                result |= tmp << shift;

                if ((byteValue & 0x80) != 0x80)
                {
                    results.Add((int) result);
                    result = 0;
                    shift = 0;
                    continue;
                }

                shift += 7;
            }

            return results;
        }

        public static byte[] GetVarintBytes(int value)
        {
            const int moreData = 128;
            var uValue = (uint) value;

            if (uValue < 0x80)
            {
                return new[] {(byte) uValue};
            }

            if (uValue < 0x4000)
            {
                return new[] {(byte) (uValue | moreData), (byte) (uValue >> 7)};
            }

            if (uValue < 0x200000)
            {
                return new[] {(byte) (uValue | moreData), (byte) ((uValue >> 7) | moreData), (byte) (uValue >> 14)};
            }

            if (uValue < 0x10000000)
            {
                return new[]
                {
                    (byte) (uValue | moreData), (byte) ((uValue >> 7) | moreData), (byte) ((uValue >> 14) | moreData),
                    (byte) (uValue >> 21)
                };
            }

            return new[]
            {
                (byte) (uValue | moreData), (byte) ((uValue >> 7) | moreData), (byte) ((uValue >> 14) | moreData),
                (byte) ((uValue >> 21) | moreData), (byte) (uValue >> 28)
            };
        }
    }
}