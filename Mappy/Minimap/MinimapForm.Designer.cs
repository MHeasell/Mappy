namespace Mappy.Minimap
{
    using Mappy.UI.Controls;

    partial class MinimapForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.minimapControl = new Mappy.Minimap.MinimapControl();
            this.SuspendLayout();
            // 
            // minimapControl
            // 
            this.minimapControl.Location = new System.Drawing.Point(0, 0);
            this.minimapControl.Margin = new System.Windows.Forms.Padding(0);
            this.minimapControl.Name = "minimapControl";
            this.minimapControl.RectColor = System.Drawing.Color.Yellow;
            this.minimapControl.Size = new System.Drawing.Size(252, 252);
            this.minimapControl.TabIndex = 1;
            this.minimapControl.Text = "minimapControl1";
            this.minimapControl.ViewportRect = new System.Drawing.Rectangle(5, 5, 50, 50);
            this.minimapControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MinimapControl1MouseDown);
            this.minimapControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MinimapControl1MouseMove);
            this.minimapControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MinimapControl1MouseUp);
            // 
            // MinimapForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(252, 252);
            this.Controls.Add(this.minimapControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MinimapForm";
            this.Text = "Minimap";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MinimapFormFormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private MinimapControl minimapControl;



    }
}