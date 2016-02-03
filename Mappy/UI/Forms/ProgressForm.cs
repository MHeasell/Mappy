namespace Mappy.UI.Forms
{
    using System;
    using System.Windows.Forms;

    using Mappy.Views;

    public partial class ProgressForm : Form, IProgressView
    {
        private bool okayToClose;

        public ProgressForm()
        {
            this.InitializeComponent();
        }

        public event EventHandler CancelPressed;

        public string Title
        {
            get
            {
                return this.Text;
            }

            set
            {
                this.Text = value;
            }
        }

        public string MessageText
        {
            get
            {
                return this.label1.Text;
            }

            set
            {
                this.label1.Text = value;
            }
        }

        public int Progress
        {
            get
            {
                return this.progressBar1.Value;
            }

            set
            {
                this.progressBar1.Value = value;
            }
        }

        public bool ShowProgress
        {
            get
            {
                return this.progressBar1.Style != ProgressBarStyle.Marquee;
            }

            set
            {
                this.progressBar1.Style = value
                    ? ProgressBarStyle.Blocks
                    : ProgressBarStyle.Marquee;
            }
        }

        public bool CancelEnabled
        {
            get
            {
                return this.button1.Enabled;
            }

            set
            {
                this.button1.Enabled = value;
            }
        }

        public void Display()
        {
            this.ShowDialog();
        }

        void IProgressView.Close()
        {
            this.okayToClose = true;
            this.Close();
        }

        private void OnCancelPressed()
        {
            this.CancelPressed?.Invoke(this, EventArgs.Empty);
        }

        private void ProgressFormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && !this.okayToClose)
            {
                this.OnCancelPressed();
                e.Cancel = true;
            }
        }

        private void CancelButtonClick(object sender, EventArgs e)
        {
            this.OnCancelPressed();
        }
    }
}
