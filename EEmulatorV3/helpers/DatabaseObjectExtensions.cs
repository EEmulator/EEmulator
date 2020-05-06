using System;
using System.Collections.Generic;
using System.Linq;
using EEmulatorV3.helpers;
using EEmulatorV3.Messages;
using ValueType = EEmulatorV3.Messages.ValueType;

namespace EEmulatorV3
{
    internal static class DatabaseObjectExtensions
    {
        public static object ToDictionary(object input)
        {
            var dictionary = new Dictionary<string, object>();

            switch (input)
            {
                case List<ObjectProperty> databaseObject:
                    foreach (var property in databaseObject)
                        dictionary.Add(property.Name, ToDictionary(property.Value));
                    break;

                case ValueObject valueObject:
                    var value = Value(valueObject);
                    if (value is List<ObjectProperty> object_properties)
                    {
                        foreach (var property in object_properties)
                            dictionary.Add(property.Name, ToDictionary(property.Value));
                    }
                    else if (value is List<ArrayProperty> array_properties)
                    {
                        var array = new object[array_properties.Count];
                        for (var i = 0; i < array_properties.Count; i++)
                            array[i] = ToDictionary(array_properties[i].Value);

                        return array;
                    }
                    else
                    {
                        return ToDictionary(value);
                    }

                    break;

                case null: return null;
                default: return input;
            }

            return dictionary;
        }

        public static List<ObjectProperty> FromDatabaseObject(DatabaseObject input)
        {
            var model = new List<ObjectProperty>();

            foreach (var kvp in input.Properties.Where(kvp => kvp.Value != null))
            {
                if (kvp.Value.GetType() == typeof(DatabaseObject))
                {
                    model.Add(new ObjectProperty() { Name = kvp.Key, Value = new ValueObject() { ValueType = ValueType.Object, ObjectProperties = FromDatabaseObject(kvp.Value as DatabaseObject) } });
                }
                else if (kvp.Value.GetType() == typeof(DatabaseArray))
                {
                    model.Add(new ObjectProperty() { Name = kvp.Key, Value = new ValueObject() { ValueType = ValueType.Array, ArrayProperties = FromDatabaseArray(kvp.Value as DatabaseArray) } });
                }
                else
                {
                    model.Add(new ObjectProperty() { Name = kvp.Key, Value = Create(kvp.Value) });
                }
            }

            return model;
        }

        public static List<ArrayProperty> FromDatabaseArray(DatabaseArray input)
        {
            var model = new List<ArrayProperty>();

            for (var i = 0; i < input.Values.Length; i++)
            {
                var value = input.Values[i];

                if (value is DatabaseArray array)
                {
                    model.AddRange(FromDatabaseArray(array));
                }
                else if (value is DatabaseObject obj)
                {
                    model.Add(new ArrayProperty() { Index = i, Value = new ValueObject() { ValueType = ValueType.Object, ObjectProperties = FromDatabaseObject(obj) } });
                }
                else
                {
                    model.Add(new ArrayProperty() { Index = i, Value = Create(value) });
                }
            }

            return model;
        }

        public static object FromDictionary(object input)
        {
            var model = new DatabaseObject();

            if (input is Dictionary<string, object>)
            {
                foreach (var kvp in input as Dictionary<string, object>)
                {
                    if (kvp.Value is Dictionary<string, object>)
                    {
                        model.SetProperty(kvp.Key, FromDictionary(kvp.Value as Dictionary<string, object>));
                    }
                    else if (kvp.Value is object[])
                    {
                        var array = new DatabaseArray();

                        foreach (var value in kvp.Value as object[])
                        {
                            array.Add(FromDictionary(value));
                        }

                        model.SetProperty(kvp.Key, array);
                    }
                    else if (kvp.Value is List<object>)
                    {
                        var array = new DatabaseArray();

                        foreach (var value in kvp.Value as List<object>)
                        {
                            array.Add(FromDictionary(value));
                        }

                        model.SetProperty(kvp.Key, array);
                    }
                    else
                    {
                        model.SetProperty(kvp.Key, kvp.Value);
                    }
                }

                return model;
            }
            else
            {
                return input;
            }
        }

        internal static List<ValueObject> MakeRange(object[] indexPath, object tail)
        {
            var result = new object[((indexPath == null) ? 0 : indexPath.Length) + ((tail == null) ? 0 : 1)];

            if (indexPath != null)
                Array.Copy(indexPath, result, indexPath.Length);

            if (tail != null)
                result[result.Length - 1] = tail;

            return result.Select(value => DatabaseObjectExtensions.Create(value)).ToList();
        }

        internal static ValueObject Create(object value)
        {
            switch (value)
            {
                case string temp: return new ValueObject { ValueType = ValueType.String, String = temp };
                case int temp: return new ValueObject { ValueType = ValueType.Int, Int = temp };
                case uint temp: return new ValueObject { ValueType = ValueType.UInt, UInt = temp };
                case long temp: return new ValueObject { ValueType = ValueType.Long, Long = temp };
                case float temp: return new ValueObject { ValueType = ValueType.Float, Float = temp };
                case double temp: return new ValueObject { ValueType = ValueType.Double, Double = temp };
                case bool temp: return new ValueObject { ValueType = ValueType.Bool, Bool = temp };
                case byte[] temp: return new ValueObject { ValueType = ValueType.ByteArray, ByteArray = temp };
                case System.DateTime temp: return new ValueObject { ValueType = ValueType.DateTime, DateTime = temp.ToUnixTime() };

                default: throw new System.ArgumentException($"The type { value.GetType().FullName } is not supported.", nameof(value));
            }
        }

        internal static object Value(this ArrayProperty property) => Value(property.Value);
        internal static object Value(this ObjectProperty property) => Value(property.Value);
        internal static object Value(ValueObject value)
        {
            switch (value.ValueType)
            {
                case ValueType.String: return value.String;
                case ValueType.Int: return value.Int;
                case ValueType.UInt: return value.UInt;
                case ValueType.Long: return value.Long;
                case ValueType.Bool: return value.Bool;
                case ValueType.Float: return value.Float;
                case ValueType.Double: return value.Double;
                case ValueType.ByteArray: return value.ByteArray;
                case ValueType.DateTime: return new DateTime(1970, 1, 1).AddMilliseconds(value.DateTime);
                case ValueType.Array: return value.ArrayProperties;
                case ValueType.Object: return value.ObjectProperties;

                default: return null;
            }
        }
    }
}
