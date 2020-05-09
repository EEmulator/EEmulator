using System;
using System.Windows.Forms;

namespace EEmulatorLauncher
{
    public partial class AddServerDialog : Form
    {
        public string Address { get; private set; }
        public decimal Port { get; private set; }

        public AddServerDialog()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Address = inputAddress.Text;
            this.Port = inputPort.Value;

            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
