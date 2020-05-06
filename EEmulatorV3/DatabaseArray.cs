using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using EEmulatorV3.Messages;

namespace EEmulatorV3
{
    public class DatabaseArray : DatabaseObject, IEnumerable<object>
    {
        internal DatabaseArray(BigDB owner, string table, string key, string version, List<ObjectProperty> properties) : base(owner, table, key, version, properties)
        {
        }

        public DatabaseArray() : base(null, string.Empty, string.Empty, string.Empty, new List<ObjectProperty>())
        {
        }

        public new object[] Values => Properties.Values.ToArray();
        public object this[uint index] => index < Values.Length - 1 ? Values[index] ?? null : throw new IndexOutOfRangeException(nameof(index));

        public DatabaseArray Set(uint index, object value) => SetProperty(index.ToString(), value) as DatabaseArray;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public DatabaseArray Add(object value) => Set((uint)this.Properties.Count, value);

        /// <summary> Add the given string value to the array. </summary>
        public DatabaseArray Add(string value) => Set((uint)this.Properties.Count, value);

        /// <summary> Add the given int value to the array. </summary>
        public DatabaseArray Add(int value) => Set((uint)this.Properties.Count, value);

        /// <summary> Add the given uint value to the array. </summary>
        public DatabaseArray Add(uint value) => Set((uint)this.Properties.Count, value);

        /// <summary> Add the given long value to the array. </summary>
        public DatabaseArray Add(long value) => Set((uint)this.Properties.Count, value);

        /// <summary> Add the given ulong value to the array. </summary>
        public DatabaseArray Add(ulong value) => Set((uint)this.Properties.Count, value);

        /// <summary> Add the given float value to the array. </summary>
        public DatabaseArray Add(float value) => Set((uint)this.Properties.Count, value);

        /// <summary> Add the given double value to the array. </summary>
        public DatabaseArray Add(double value) => Set((uint)this.Properties.Count, value);

        /// <summary> Add the given boolean value to the array. </summary>
        public DatabaseArray Add(bool value) => Set((uint)this.Properties.Count, value);

        /// <summary> Add the given byte array value to the array. </summary>
        public DatabaseArray Add(byte[] value) => Set((uint)this.Properties.Count, value);

        /// <summary> Add the given date time value to the array. </summary>
        public DatabaseArray Add(DateTime value) => Set((uint)this.Properties.Count, value);

        /// <summary> Add the given object to the array. </summary>
        public DatabaseArray Add(DatabaseObject value) => Set((uint)this.Properties.Count, value);

        /// <summary> Add the given array to the array. </summary>
        public DatabaseArray Add(DatabaseArray value) => Set((uint)this.Properties.Count, value);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override DatabaseObject SetProperty(string index, object value)
        {
            if (!int.TryParse(index, out int i))
                throw new Exception("You must specify the index as an integer.");

            for (var j = this.Properties.Count; j < i; j++)
                base.SetProperty(j.ToString(), null);

            return base.SetProperty(index, value);
        }

        public bool GetBool(uint index) => GetBool(index.ToString());
        public bool GetBool(uint index, bool defaultValue) => GetBool(index.ToString(), defaultValue);

        public byte[] GetBytes(uint index) => GetBytes(index.ToString());
        public byte[] GetBytes(uint index, byte[] defaultValue) => GetBytes(index.ToString(), defaultValue);

        public double GetDouble(uint index) => GetDouble(index.ToString());
        public double GetDouble(uint index, double defaultValue) => GetDouble(index.ToString(), defaultValue);

        public float GetFloat(uint index) => GetFloat(index.ToString());
        public float GetFloat(uint index, float defaultValue) => GetFloat(index.ToString(), defaultValue);

        public int GetInt(uint index) => GetInt(index.ToString());
        public int GetInt(uint index, int defaultValue) => GetInt(index.ToString(), defaultValue);

        public uint GetUInt(uint index) => GetUInt(index.ToString());
        public uint GetUInt(uint index, uint defaultValue) => GetUInt(index.ToString(), defaultValue);

        public long GetLong(uint index) => GetLong(index.ToString());
        public long GetLong(uint index, long defaultValue) => GetLong(index.ToString(), defaultValue);

        public string GetString(uint index) => GetString(index.ToString());
        public string GetString(uint index, string defaultValue) => GetString(index.ToString(), defaultValue);

        public DatabaseObject GetObject(uint index) => GetObject(index.ToString());
        public DatabaseObject GetObject(uint index, DatabaseObject defaultValue) => GetObject(index.ToString(), defaultValue);

        public DatabaseArray GetArray(uint index) => GetArray(index.ToString());
        public DatabaseArray GetArray(uint index, DatabaseArray defaultValue) => GetArray(index.ToString(), defaultValue);

        public new IEnumerator<object> GetEnumerator() => ((IEnumerable<object>)Values).GetEnumerator();
    }
}
