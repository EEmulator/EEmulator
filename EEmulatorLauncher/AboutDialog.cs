using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace EEmulatorLauncher
{
    public partial class AboutDialog : Form
    {
        public AboutDialog()
        {
            InitializeComponent();
        }

        private void linkForums_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://forums.everybodyedits.com/viewtopic.php?id=46938");
        }

        private void linkSource_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/EEmulator/EEmulator");
        }

        private void pbHeart_Click(object sender, EventArgs e)
        {
            pbConfetti.Visible = true;

            new Thread(() => {
                var hWnd = this.Handle;
                var r = new Random();
                for (var i = 0; i < 200; i++)
                {
                    var offset = 6;
                    var rct = new RECT();
                    GetWindowRect(hWnd, ref rct);

                    var currentX = rct.Left;
                    var currentY = rct.Top;
                    var x = r.Next(currentX - offset, currentX + offset + 1);
                    var y = r.Next(currentY - offset, currentY + offset + 1);

                    SetWindowPos(hWnd, 0, x, y, currentX, currentY, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);

                    Thread.Sleep(25);
                }
                
                pbConfetti.Visible = false;
            }).Start();

        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        const short SWP_NOMOVE = 0X2;
        const short SWP_NOSIZE = 1;
        const short SWP_NOZORDER = 0X4;
        const int SWP_SHOWWINDOW = 0x0040;
    }
}
