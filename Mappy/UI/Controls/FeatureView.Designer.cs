namespace Mappy.UI.Controls
{
    partial class FeatureView
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.control = new Mappy.UI.Controls.DoubleComboListView();
            this.SuspendLayout();
            // 
            // control
            // 
            this.control.Dock = System.Windows.Forms.DockStyle.Fill;
            this.control.Location = new System.Drawing.Point(0, 0);
            this.control.Name = "control";
            this.control.Size = new System.Drawing.Size(206, 517);
            this.control.TabIndex = 0;
            // 
            // FeatureView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.control);
            this.Name = "FeatureView";
            this.Size = new System.Drawing.Size(206, 517);
            this.ResumeLayout(false);

        }

        #endregion

        private DoubleComboListView control;
    }
}
