using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EEmulatorV3.Messages;
using Tson.NET;

namespace EEmulatorV3
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
                if (output.Count() < limit)
                    return output;

                var dbo = DatabaseObject.LoadFromString(File.ReadAllText(file));
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