using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using DarkUI.Controls;
using EEmulatorLauncher.Properties;
using Flurl.Http;
using Newtonsoft.Json;

namespace EEmulatorLauncher
{
    public partial class ServerForm : Form
    {
        public class RoomInfo
        {
            public string Id { get; set; }
            public string RoomType { get; set; }
            public int OnlineUsers { get; set; }
            public List<KeyValuePair> RoomData { get; set; }
        }

        public class KeyValuePair
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public class GameInfo
        {
            public string GameId { get; set; }
            public List<RoomInfo> Rooms { get; set; }
        }

        public class ServerEntry
        {
            public string EndPoint { get; set; }
        }

        public Dictionary<string, string> Descriptions = new Dictionary<string, string>();

        public ServerForm()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();

            if (File.Exists("servers.json"))
            {
                try
                {
                    var entries = JsonConvert.DeserializeObject<List<ServerEntry>>(File.ReadAllText("servers.json"));

                    foreach (var entry in entries)
                    {
                        ddServers.Items.Add(new DarkDropdownItem() { Text = entry.EndPoint });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to load server list, was it corrupted? " + ex.Message);
                }
            }
        }

        private void ServerForm_Load(object sender, EventArgs e)
        {
        }

        private void refreshLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var endpoint = ddServers.SelectedItem.Text;
                var statusResponse = $"http://{endpoint.Split(':')[0]}:80/status".GetAsync().ReceiveString().Result;
                this.UpdateDescription();
            }
            catch
            {
                textDescription.Text = "Offline.";
            }
        }

        private void btnAddServer_Click(object sender, EventArgs e)
        {
            var dialog = new AddServerDialog();
            var statusResponse = "";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var endpoint = dialog.Address + ":" + dialog.Port;

                if (ddServers.Items.Any(x => x.Text == endpoint))
                {
                    MessageBox.Show("A server has already been added with the endpoint specified.");
                    return;
                }

                try
                {
                    statusResponse = $"http://{endpoint.Split(':')[0]}:80/status".GetAsync().ReceiveString().Result;
                    Descriptions.Add(endpoint, statusResponse);
                    ddServers.Items.Add(new DarkDropdownItem() { Text = endpoint });
                    this.UpdateDescription();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to add server - is it responding?\n" + ex.Message + "\n" + "Status Response: " + statusResponse);
                }
            }
        }

        private void btnPlayNow_Click(object sender, EventArgs e)
        {
            if (ddServers.SelectedItem == null)
            {
                MessageBox.Show("No server selected. You must select a server to join.");
                return;
            }

            try
            {
                var endpoint = ddServers.SelectedItem.Text;
                var response = ($"http://{ endpoint.Split(':')[0]}:80/rooms".GetAsync().ReceiveJson<List<GameInfo>>().Result);

                // TODO: For certain games without lobbies, display a list of rooms to join.

                if (Process.GetProcessesByName("EEmulator").Any())
                {
                    MessageBox.Show("An instance of EEmulator is already running - currently, only one can run at a time.");
                    return;
                }

                var version = "unspecified";

                foreach (var game in response)
                {
                    switch (game.GameId)
                    {
                        case "everybody-edits-v5":
                            version = "v0500";
                            break;

                        case "everybody-edits-v7":
                            version = "v0700";
                            break;

                        case "everybody-edits-v8":
                            version = "v0800";
                            break;

                        case "everybody-edits-v89":
                            version = "v89";
                            break;

                        case "everybody-edits-v188":
                            version = "v188";
                            break;

                        case "everybody-edits-v225":
                            version = "v225";
                            break;

                        default:
                            MessageBox.Show("The server is not running an instance of an Everybody Edits game.");
                            return;
                    }

                    if (version != "unspecified")
                    {
                        Process.Start("EEmulator.exe", $"EverybodyEdits {version} " + endpoint.Split(':')[0] + ":8184").WaitForExit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to connect to the server. " + ex.Message);
            }
        }

        private void ddServers_SelectedItemChanged(object sender, EventArgs e)
        {
            this.UpdateDescription();
        }

        private void UpdateDescription()
        {
            if (ddServers.SelectedItem == null)
                return;

            try
            {
                var description = this.Descriptions[ddServers.SelectedItem.Text];
                textDescription.Text = description;
            }
            catch
            {
                textDescription.Text = "";
            }
        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var entries = ddServers.Items.Distinct().Select(x => new ServerEntry() { EndPoint = x.Text }).ToList();
            File.WriteAllText("servers.json", JsonConvert.SerializeObject(entries));
        }
    }
}
