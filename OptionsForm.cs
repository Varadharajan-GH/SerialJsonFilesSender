using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SerialJsonFilesSender
{
    public partial class OptionsForm: Form
    {
        public int FileDelayMs { get; private set; }
        public int ResendIntervalMs { get; private set; }
        public int WriteTimeOutMs { get; private set; }
        public int BaudRate { get; set; }

        public OptionsForm(int fileDelay, int resendInterval, int writeTimeOutMs, int baudRate)
        {
            InitializeComponent();
            FileDelayMs = fileDelay;
            ResendIntervalMs = resendInterval;
            WriteTimeOutMs = writeTimeOutMs;
            txtFileDelay.Text = FileDelayMs.ToString();
            txtDirDelay.Text = ResendIntervalMs.ToString();
            txtWriteTimeout.Text = WriteTimeOutMs.ToString();
            BaudRate = baudRate;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                FileDelayMs = int.Parse(txtFileDelay.Text);
                ResendIntervalMs = int.Parse(txtDirDelay.Text);
                WriteTimeOutMs = int.Parse(txtWriteTimeout.Text);
                BaudRate = int.Parse(txtBaudRate.Text);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid input.");
                lblInfo.Text = ex.Message;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
