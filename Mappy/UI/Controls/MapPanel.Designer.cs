namespace Mappy.UI.Controls
{
    partial class MapPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mapControl = new ImageLayerView();
            this.SuspendLayout();
            // 
            // mapControl
            // 
            this.mapControl.AllowDrop = true;
            this.mapControl.BackColor = System.Drawing.Color.Navy;
            this.mapControl.Location = new System.Drawing.Point(0, 0);
            this.mapControl.Name = "mapControl";
            this.mapControl.Size = new System.Drawing.Size(150, 150);
            this.mapControl.TabIndex = 0;
            this.mapControl.Text = "mapControl1";
            // 
            // MapPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.CornflowerBlue;
            this.Controls.Add(this.mapControl);
            this.Name = "MapPanel";
            this.ResumeLayout(false);

        }

        #endregion

        private ImageLayerView mapControl;
    }
}
