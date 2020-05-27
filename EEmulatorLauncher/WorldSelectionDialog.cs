using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EEmulator;
using EEWorldArchive;

namespace EEmulatorLauncher
{
    public partial class WorldSelectionDialog : Form
    {
        public List<WorldArchive.World> Worlds { get; set; }
        public WorldArchive.World SelectedWorld { get; set; }
        public bool HasEnumeratedArchive { get; set; }

        public WorldSelectionDialog()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (!Program.UsernameToConnectUserId.ContainsKey(txtUsername.Text.ToLower()))
            {
                MessageBox.Show("The user could not be found - if this an error and not a misspelling, you may report it on the forum topic.", "Unable to locate user.", MessageBoxButtons.OK);
                return;
            }

            if (!this.HasEnumeratedArchive)
            {
                MessageBox.Show("Please note that the archive is quite large, so it may take a minute or two the first time you load a user. After that, it should be considerably fast as it is loaded into memory.");
            }

            this.Worlds = Program.WorldArchive.Retrieve(Program.UsernameToConnectUserId[txtUsername.Text.ToLower()]).ToList();
            comboWorlds.Items.Clear();
            comboWorlds.Items.AddRange(this.Worlds.Select(world => new ComboWorldItem() { Text = world.Object.GetString("name", "Untitled World") + " (" + world.WorldId + ")", WorldId = world.WorldId }).ToArray());
        
            if (!this.HasEnumeratedArchive)
            {
                MessageBox.Show("The worlds have been loaded!");
                this.HasEnumeratedArchive = true;
            }
        }

        private void comboWorlds_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.Worlds == null)
                return;

            if (comboWorlds.Items.Count >= 1)
            {
                if (comboWorlds.SelectedIndex >= 0 && comboWorlds.SelectedIndex <= comboWorlds.Items.Count)
                {
                    if (comboWorlds.SelectedItem == null)
                        return;

                    var selectedItem = (ComboWorldItem)comboWorlds.SelectedItem;
                    var world = this.Worlds.Find(w => w.WorldId == selectedItem.WorldId);

                    this.SelectedWorld = world;
                    pbMinimap.Size = world.Minimap.Size;
                    txtDetails.Text = world.Tson;
                    pbMinimap.Image = world.Minimap;

                    // TODO: Show estimated version and make default in combo box.
                    switch (this.EstimateVersionFromBlocks())
                    {
                    }
                }
            }
        }

        private void btnPlayNow_Click(object sender, EventArgs e)
        {
            if (comboSelectedVersion.SelectedItem == null)
                return;

            switch ((string)comboSelectedVersion.SelectedItem)
            {
                case "Everybody Edits v225":
                {
                    var gameId = new EEmulator.EverybodyEdits(EverybodyEditsVersion.v225).GameId;
                    var destination = Path.Combine("games", "EverybodyEdits", "bigdb", gameId, "Worlds", this.SelectedWorld.WorldId + ".tson");

                    if (!File.Exists(destination))
                        File.WriteAllText(destination, this.SelectedWorld.Tson);

                    Process.Start("EEmulator.exe", "EverybodyEdits v225 localhost:8184 " + this.SelectedWorld.WorldId);
                    break;
                }

                case "Everybody Edits v188":
                {
                    var gameId = new EEmulator.EverybodyEdits(EverybodyEditsVersion.v188).GameId;
                    var destination = Path.Combine("games", "EverybodyEdits", "bigdb", gameId, "Worlds", this.SelectedWorld.WorldId + ".tson");

                    if (!File.Exists(destination))
                        File.WriteAllText(destination, this.SelectedWorld.Tson);

                    Process.Start("EEmulator.exe", "EverybodyEdits v188 localhost:8184 " + this.SelectedWorld.WorldId);
                    break;
                }

                case "Everybody Edits v89":
                {
                    var gameId = new EEmulator.EverybodyEdits(EverybodyEditsVersion.v89).GameId;
                    var destination = Path.Combine("games", "EverybodyEdits", "bigdb", gameId, "Worlds", this.SelectedWorld.WorldId + ".tson");

                    if (!File.Exists(destination))
                        File.WriteAllText(destination, this.SelectedWorld.Tson);

                    Process.Start("EEmulator.exe", "EverybodyEdits v89 localhost:8184 " + this.SelectedWorld.WorldId);
                    break;
                }

                case "0.8.0.0":
                {
                    var gameId = new EEmulator.EverybodyEdits(EverybodyEditsVersion.v0800).GameId;
                    var destination = Path.Combine("games", "EverybodyEdits", "bigdb", gameId, "Worlds", this.SelectedWorld.WorldId + ".tson");

                    if (!File.Exists(destination))
                        File.WriteAllText(destination, this.SelectedWorld.Tson);

                    Process.Start("EEmulator.exe", "EverybodyEdits v0800 localhost:8184 " + this.SelectedWorld.WorldId);
                    break;
                }

                case "0.7.0.0":
                {
                    var gameId = new EEmulator.EverybodyEdits(EverybodyEditsVersion.v0700).GameId;
                    var destination = Path.Combine("games", "EverybodyEdits", "bigdb", gameId, "Worlds", this.SelectedWorld.WorldId + ".tson");

                    if (!File.Exists(destination))
                        File.WriteAllText(destination, this.SelectedWorld.Tson);

                    Process.Start("EEmulator.exe", "EverybodyEdits v0700 localhost:8184 " + this.SelectedWorld.WorldId);
                    break;
                }

                case "0.5.0.0":
                {
                    var gameId = new EEmulator.EverybodyEdits(EverybodyEditsVersion.v0500).GameId;
                    var destination = Path.Combine("games", "EverybodyEdits", "bigdb", gameId, "Worlds", this.SelectedWorld.WorldId + ".tson");

                    if (!File.Exists(destination))
                        File.WriteAllText(destination, this.SelectedWorld.Tson);

                    Process.Start("EEmulator.exe", "EverybodyEdits v0500 localhost:8184 " + this.SelectedWorld.WorldId);
                    break;
                }
            }
        }

        public EverybodyEditsVersion EstimateVersionFromBlocks()
        {
            if (this.SelectedWorld == null)
                return EverybodyEditsVersion.v188; // default to latest

            if (this.SelectedWorld.BlockTypes.Count() == 0)
                return EverybodyEditsVersion.v188; // default to latest

            var largest_type = this.SelectedWorld.BlockTypes.Max(); // TODO: recommend from largest block type.
            return EverybodyEditsVersion.v188;
        }
    }

    public class ComboWorldItem
    {
        public string Text { get; set; }
        public string WorldId { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
