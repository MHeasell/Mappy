namespace Mappy.UI.Forms
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    using Mappy.Views;

    public partial class ProgressForm : Form, IProgressView
    {
        public ProgressForm()
        {
            this.InitializeComponent();
        }

        public event EventHandler CancelPressed
        {
            add
            {
                this.button1.Click += value;
            }

            remove
            {
                this.button1.Click -= value;
            }
        }

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
    }
}
