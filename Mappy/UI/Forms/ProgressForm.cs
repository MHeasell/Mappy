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
            EventHandler h = this.CancelPressed;
            if (h != null)
            {
                h(this, EventArgs.Empty);
            }
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
