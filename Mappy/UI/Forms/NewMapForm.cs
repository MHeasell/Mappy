namespace Mappy.UI.Forms
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    public partial class NewMapForm : Form
    {
        public NewMapForm()
        {
            this.InitializeComponent();
        }

        public int MapWidth
        {
            get; private set;
        }

        public int MapHeight
        {
            get; private set;
        }

        private void FormValidating(object sender, CancelEventArgs e)
        {
            try
            {
                var w = Convert.ToInt32(this.widthTextBox.Text);
                var h = Convert.ToInt32(this.heightTextBox.Text);

                if (w < 1 || h < 1)
                {
                    e.Cancel = true;
                }
                else
                {
                    this.MapWidth = w;
                    this.MapHeight = h;
                }
            }
            catch (FormatException)
            {
                e.Cancel = true;
            }
        }

        private bool ValidateFields()
        {
            try
            {
                var w = Convert.ToInt32(this.widthTextBox.Text);
                var h = Convert.ToInt32(this.heightTextBox.Text);

                if (w < 1 || h < 1)
                {
                    return false;
                }
                else
                {
                    this.MapWidth = w;
                    this.MapHeight = h;
                }
            }
            catch (FormatException)
            {
                return false;
            }

            return true;
        }

        private void Button1Click(object sender, EventArgs e)
        {
            if (this.ValidateFields())
            {
                this.DialogResult = DialogResult.OK;
            }
        }

        private void WidthTextChanged(object sender, EventArgs e)
        {
            int valX;
            if (!int.TryParse(this.widthTextBox.Text, out valX))
            {
                this.convertedWidthLabel.Text = string.Empty;
                return;
            }

            var convertedX = (valX * 32) / 512.0f;

            this.convertedWidthLabel.Text = $"({convertedX})";
        }

        private void HeightTextChanged(object sender, EventArgs e)
        {
            int valY;
            if (!int.TryParse(this.heightTextBox.Text, out valY))
            {
                this.convertedHeightLabel.Text = string.Empty;
                return;
            }

            var convertedY = (valY * 32) / 512.0f;

            this.convertedHeightLabel.Text = $"({convertedY})";
        }
    }
}
