using System;
using System.Diagnostics;
using System.Windows.Forms;
using EEmulatorLauncher.Properties;

namespace EEmulatorLauncher
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
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
            MessageBox.Show("To use, log in with guest.", "Before you start.", MessageBoxButtons.OK);
            Process.Start("EEmulator.exe", "EverybodyEdits v0500").WaitForExit();
        }

        private void btnPlay0700_Click(object sender, EventArgs e)
        {
            MessageBox.Show("To use, log in with guest.", "Before you start.", MessageBoxButtons.OK);
            Process.Start("EEmulator.exe", "EverybodyEdits v0700").WaitForExit();
        }

        private void btnPlay0800_Click(object sender, EventArgs e)
        {
            MessageBox.Show("To use, log in with email as 'guest' without quotation marks, any password will work.", "Before you start.", MessageBoxButtons.OK);
            Process.Start("EEmulator.exe", "EverybodyEdits v0800").WaitForExit();
        }

        private void btnPlay89_Click(object sender, EventArgs e)
        {
            MessageBox.Show("To use, log in with email as \"user\" without quotes, any password will work.", "Before you start.", MessageBoxButtons.OK);
            Process.Start("EEmulator.exe", "EverybodyEdits v89").WaitForExit();
        }

        private void btnPlay188_Click(object sender, EventArgs e)
        {
            Process.Start("EEmulator.exe", "EverybodyEdits v188").WaitForExit();
        }
    }
}
