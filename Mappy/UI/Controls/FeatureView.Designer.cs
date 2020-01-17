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
			this.panel1 = new System.Windows.Forms.Panel();
			this.SelectionBtn = new System.Windows.Forms.RadioButton();
			this.SporadicBtn = new System.Windows.Forms.RadioButton();
			this.FillBtn = new System.Windows.Forms.RadioButton();
			this.LineBtn = new System.Windows.Forms.RadioButton();
			this.magnitude = new System.Windows.Forms.NumericUpDown();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.magnitude)).BeginInit();
			this.SuspendLayout();
			// 
			// control
			// 
			this.control.Padding = new System.Windows.Forms.Padding(3, 33, 3, 3);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.magnitude);
			this.panel1.Controls.Add(this.SelectionBtn);
			this.panel1.Controls.Add(this.SporadicBtn);
			this.panel1.Controls.Add(this.FillBtn);
			this.panel1.Controls.Add(this.LineBtn);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.MaximumSize = new System.Drawing.Size(0, 30);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(202, 30);
			this.panel1.TabIndex = 1;
			// 
			// SelectionBtn
			// 
			this.SelectionBtn.Appearance = System.Windows.Forms.Appearance.Button;
			this.SelectionBtn.AutoSize = true;
			this.SelectionBtn.Checked = true;
			this.SelectionBtn.Location = new System.Drawing.Point(3, 3);
			this.SelectionBtn.Name = "SelectionBtn";
			this.SelectionBtn.Size = new System.Drawing.Size(32, 23);
			this.SelectionBtn.TabIndex = 3;
			this.SelectionBtn.TabStop = true;
			this.SelectionBtn.Text = "Sel";
			this.SelectionBtn.UseVisualStyleBackColor = true;
			this.SelectionBtn.MouseClick += new System.Windows.Forms.MouseEventHandler(this.SelectionBtn_MouseClick);
			// 
			// SporadicBtn
			// 
			this.SporadicBtn.Appearance = System.Windows.Forms.Appearance.Button;
			this.SporadicBtn.AutoSize = true;
			this.SporadicBtn.Location = new System.Drawing.Point(104, 4);
			this.SporadicBtn.Name = "SporadicBtn";
			this.SporadicBtn.Size = new System.Drawing.Size(59, 23);
			this.SporadicBtn.TabIndex = 2;
			this.SporadicBtn.TabStop = true;
			this.SporadicBtn.Text = "Sporadic";
			this.SporadicBtn.UseVisualStyleBackColor = true;
			this.SporadicBtn.MouseClick += new System.Windows.Forms.MouseEventHandler(this.SporadicBtn_MouseClick);
			// 
			// FillBtn
			// 
			this.FillBtn.Appearance = System.Windows.Forms.Appearance.Button;
			this.FillBtn.AutoSize = true;
			this.FillBtn.Location = new System.Drawing.Point(74, 3);
			this.FillBtn.Name = "FillBtn";
			this.FillBtn.Size = new System.Drawing.Size(29, 23);
			this.FillBtn.TabIndex = 1;
			this.FillBtn.TabStop = true;
			this.FillBtn.Text = "Fill";
			this.FillBtn.UseVisualStyleBackColor = true;
			this.FillBtn.MouseClick += new System.Windows.Forms.MouseEventHandler(this.FillBtn_MouseClick);
			// 
			// LineBtn
			// 
			this.LineBtn.Appearance = System.Windows.Forms.Appearance.Button;
			this.LineBtn.AutoSize = true;
			this.LineBtn.Location = new System.Drawing.Point(36, 3);
			this.LineBtn.Name = "LineBtn";
			this.LineBtn.Size = new System.Drawing.Size(37, 23);
			this.LineBtn.TabIndex = 0;
			this.LineBtn.TabStop = true;
			this.LineBtn.Text = "Line";
			this.LineBtn.UseVisualStyleBackColor = true;
			this.LineBtn.MouseClick += new System.Windows.Forms.MouseEventHandler(this.LineBtn_MouseClick);
			// 
			// magnitude
			// 
			this.magnitude.Location = new System.Drawing.Point(165, 5);
			this.magnitude.Name = "magnitude";
			this.magnitude.Size = new System.Drawing.Size(36, 20);
			this.magnitude.TabIndex = 2;
			this.magnitude.ValueChanged += new System.EventHandler(this.Magnitude_ValueChanged);
			// 
			// FeatureView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.Name = "FeatureView";
			this.Controls.SetChildIndex(this.control, 0);
			this.Controls.SetChildIndex(this.panel1, 0);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.magnitude)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton SporadicBtn;
        private System.Windows.Forms.RadioButton FillBtn;
        private System.Windows.Forms.RadioButton LineBtn;
		private System.Windows.Forms.RadioButton SelectionBtn;
		private System.Windows.Forms.NumericUpDown magnitude;
	}
}
