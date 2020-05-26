using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EEmulator.helpers;
using EEmulator.Messages;
using Tson;

namespace EEmulator
{
    public class BigDB
    {
        public IGame Game { get; }
        string StorageLocation { get; }

        public BigDB(IGame game)
        {
            this.Game = game;
            this.StorageLocation = Path.Combine("games", "EverybodyEdits", "bigdb", this.Game.GameId);

            Directory.CreateDirectory(this.StorageLocation);
        }

        public void CreateTable(string table)
        {
            Directory.CreateDirectory(Path.Combine(this.StorageLocation, table));
        }

        public void CreateObject(string table, string key, DatabaseObject obj)
        {
            File.WriteAllText(Path.Combine(this.StorageLocation, table, key + ".tson"), TsonConvert.SerializeObject(DatabaseObjectExtensions.ToDictionary(obj.Properties), Formatting.Indented));
        }

        public List<DatabaseObject> LoadMatchingObjects(string table, string index, List<ValueObject> indexValue, int limit)
        {
            if (!this.TableExists(table))
                throw new Exception($"Table '{table}' does not exist.");

            var correctTable = this.CorrectTableCase(table);
            var output = new List<DatabaseObject>();

            foreach (var file in Directory.GetFiles(Path.Combine(this.StorageLocation, correctTable), "*.tson", SearchOption.TopDirectoryOnly))
            {
                if (output.Count() >= limit)
                    return output;

                var dbo = DatabaseObject.LoadFromString(File.ReadAllText(file));

                dbo.Key = Path.GetFileNameWithoutExtension(file);

                if (!dbo.ContainsKey(index))
                    continue;

                var match = false;
                foreach (var value in indexValue)
                {
                    switch (value.ValueType)
                    {
                        case Messages.ValueType.String:
                            if (dbo[index] is string)
                                if (dbo[index] as string == value.String)
                                    match = true;
                            break;

                        case Messages.ValueType.Int:
                            if (dbo[index] is int)
                                if ((int)dbo[index] == value.Int)
                                    match = true;
                            break;

                        case Messages.ValueType.UInt:
                            if (dbo[index] is uint)
                                if ((uint)dbo[index] == value.UInt)
                                    match = true;
                            break;

                        case Messages.ValueType.Long:
                            if (dbo[index] is long)
                                if ((long)dbo[index] == value.Long)
                                    match = true;
                            break;

                        case Messages.ValueType.Bool:
                            if (dbo[index] is bool)
                                if ((bool)dbo[index] == value.Bool)
                                    match = true;
                            break;

                        case Messages.ValueType.Float:
                            if (dbo[index] is float)
                                if ((float)dbo[index] == value.Float)
                                    match = true;
                            break;

                        case Messages.ValueType.Double:
                            if (dbo[index] is double)
                                if ((double)dbo[index] == value.Double)
                                    match = true;
                            break;

                        case Messages.ValueType.ByteArray:
                            if (dbo[index] is byte[])
                                if ((byte[])dbo[index] == value.ByteArray)
                                    match = true;
                            break;

                        case Messages.ValueType.DateTime:
                            if (dbo[index] is DateTime)
                                if (((DateTime)dbo[index]).ToUnixTime() == value.DateTime)
                                    match = true;
                            break;

                        case Messages.ValueType.Array:
                        case Messages.ValueType.Object:
                            throw new NotImplementedException();
                    }
                }

                if (match)
                {
                    output.Add(dbo);
                }

                output.Add(dbo);
            }

            return output;
        }

        public DatabaseObject Load(string table, string key)
        {
            if (!this.TableExists(table))
                throw new Exception($"Table '{table}' does not exist.");

            var (exists, obj) = FindObjectIfExists(table, key);
            return (exists) ? obj : throw new Exception($"Table '{table}' does not contain any object with the key '{key}'.");
        }

        public List<DatabaseObject> LoadRange(string table, string index, List<ValueObject> startIndexValue, List<ValueObject> stopIndexValue, int limit)
        {
            if (!this.TableExists(table))
                throw new Exception($"Table '{table}' does not exist.");

            var correctTable = this.CorrectTableCase(table);
            var output = new List<DatabaseObject>();

            // TODO: Needs implemented.
            foreach (var file in Directory.GetFiles(Path.Combine(this.StorageLocation, correctTable), "*.tson", SearchOption.TopDirectoryOnly))
            {
                if (output.Count() >= limit)
                    return output;

                var dbo = DatabaseObject.LoadFromString(File.ReadAllText(file));
                dbo.Key = Path.GetFileNameWithoutExtension(file);
                output.Add(dbo);
            }

            return output;
        }

        private bool TableExists(string table) => Directory.GetDirectories(this.StorageLocation, "*", SearchOption.TopDirectoryOnly).Select(t => new DirectoryInfo(t)).Any(x => x.Name.ToLower() == table.ToLower());
        private string CorrectTableCase(string table) => Directory.GetDirectories(this.StorageLocation, "*", SearchOption.TopDirectoryOnly).Select(t => new DirectoryInfo(t)).First(x => x.Name.ToLower() == table.ToLower()).Name;

        public (bool exists, DatabaseObject obj) FindObjectIfExists(string table, string key)
            => File.Exists(Path.Combine(this.StorageLocation, this.CorrectTableCase(table), key + ".tson"))
                ? (true, DatabaseObject.LoadFromString(File.ReadAllText(Path.Combine(this.StorageLocation, this.CorrectTableCase(table), key + ".tson"))))
                : ((bool exists, DatabaseObject obj))(false, null);
    }
}