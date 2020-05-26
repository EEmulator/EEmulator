using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using EEmulatorLauncher.Properties;

namespace EEmulatorLauncher
{
    public partial class MainForm : Form
    {
        private ServerForm serverForm;

        public MainForm()
        {
            serverForm = new ServerForm();
            InitializeComponent();
        }

        private void OnBirbClick(object sender, EventArgs e)
        {
            MessageBox.Show("Chirp!\nEverybody Edits Emulator was made by atillabyte (miou) and EEJesse.\nYou can find it at: https://github.com/EEmulator");
        }

        private void btnPlay0500_MouseHover(object sender, EventArgs e)
        {
            bottomPanel.BackgroundImage = Resources.bg_0500;
        }

        private void btnPlay0700_MouseHover(object sender, EventArgs e)
        {
            bottomPanel.BackgroundImage = Resources.bg_0700;
        }

        private void btnPlay0800_MouseHover(object sender, EventArgs e)
        {
            bottomPanel.BackgroundImage = Resources.bg_0800;
        }

        private void btnPlay89_MouseHover(object sender, EventArgs e)
        {
            bottomPanel.BackgroundImage = Resources.bg_89;
        }

        private void btnPlay188_MouseHover(object sender, EventArgs e)
        {
            bottomPanel.BackgroundImage = Resources.bg_188;
        }

        private void btnPlay0500_Click(object sender, EventArgs e)
        {
            Process.Start("EEmulator.exe", "EverybodyEdits v0500 localhost:8184 null").WaitForExit();
        }

        private void btnPlay0700_Click(object sender, EventArgs e)
        {
            Process.Start("EEmulator.exe", "EverybodyEdits v0700 localhost:8184 null").WaitForExit();
        }

        private void btnPlay0800_Click(object sender, EventArgs e)
        {
            MessageBox.Show("To use, log in with email as 'guest' without quotation marks, any password will work.", "Before you start.", MessageBoxButtons.OK);
            Process.Start("EEmulator.exe", "EverybodyEdits v0800 localhost:8184 null").WaitForExit();
        }

        private void btnPlay89_Click(object sender, EventArgs e)
        {
            MessageBox.Show("To use, log in with email as \"user\" without quotes, any password will work.", "Before you start.", MessageBoxButtons.OK);
            Process.Start("EEmulator.exe", "EverybodyEdits v89 localhost:8184 null").WaitForExit();
        }

        private void btnPlay188_Click(object sender, EventArgs e)
        {
            Process.Start("EEmulator.exe", "EverybodyEdits v188 localhost:8184 PW01").WaitForExit();
        }

        private void btnNetwork_Click(object sender, EventArgs e)
        {
            serverForm.ShowDialog();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void btnWorldGallery_Click(object sender, EventArgs e)
        {
            if (!File.Exists(@"inc/worlds.7z"))
            {
                if (MessageBox.Show("Unable to locate world archive. (inc/worlds.7z)\n Do you wish to download it now?", "Unable to locate world archive.", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Process.Start("https://archive.org/details/EEWorlds");
                    MessageBox.Show("You should be redirected to https://archive.org/details/EEWorlds \nPlease download it and place it in the following folder:\n" + Path.Combine(Application.StartupPath, "inc"));
                }

                return;
            }

            if (!File.Exists(@"inc/colors.json"))
            {
                MessageBox.Show("Unable to locate colors file. (inc/colors.json)");
                return;
            }

            Program.WorldArchive = new EEWorldArchive.WorldArchive(@"inc/worlds.7z");
            new WorldSelectionDialog().Show();
        }
    }
}
