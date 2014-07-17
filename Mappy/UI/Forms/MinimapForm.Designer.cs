namespace Mappy.UI.Forms
{
    using Controls;

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
            this.minimapControl1 = new MinimapControl();
            this.SuspendLayout();
            // 
            // minimapControl1
            // 
            this.minimapControl1.Location = new System.Drawing.Point(0, 0);
            this.minimapControl1.Margin = new System.Windows.Forms.Padding(0);
            this.minimapControl1.Name = "minimapControl1";
            this.minimapControl1.RectColor = System.Drawing.Color.Yellow;
            this.minimapControl1.Size = new System.Drawing.Size(252, 252);
            this.minimapControl1.TabIndex = 1;
            this.minimapControl1.Text = "minimapControl1";
            this.minimapControl1.ViewportRect = new System.Drawing.Rectangle(5, 5, 50, 50);
            this.minimapControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MinimapControl1MouseDown);
            this.minimapControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MinimapControl1MouseMove);
            this.minimapControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MinimapControl1MouseUp);
            // 
            // MinimapForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(252, 252);
            this.Controls.Add(this.minimapControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MinimapForm";
            this.Text = "Minimap";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MinimapFormFormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        public MinimapControl minimapControl1;


    }
}