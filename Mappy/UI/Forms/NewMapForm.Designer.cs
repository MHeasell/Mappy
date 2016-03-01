namespace Mappy.UI.Forms
{
    partial class NewMapForm
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
            this.widthTextBox = new System.Windows.Forms.TextBox();
            this.widthLabel = new System.Windows.Forms.Label();
            this.heightLabel = new System.Windows.Forms.Label();
            this.heightTextBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.convertedWidthLabel = new System.Windows.Forms.Label();
            this.convertedHeightLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // widthTextBox
            // 
            this.widthTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.widthTextBox.Location = new System.Drawing.Point(86, 12);
            this.widthTextBox.Name = "widthTextBox";
            this.widthTextBox.Size = new System.Drawing.Size(53, 20);
            this.widthTextBox.TabIndex = 0;
            this.widthTextBox.Text = "64";
            this.widthTextBox.TextChanged += new System.EventHandler(this.WidthTextChanged);
            // 
            // widthLabel
            // 
            this.widthLabel.AutoSize = true;
            this.widthLabel.Location = new System.Drawing.Point(14, 15);
            this.widthLabel.Name = "widthLabel";
            this.widthLabel.Size = new System.Drawing.Size(66, 13);
            this.widthLabel.TabIndex = 1;
            this.widthLabel.Text = "Width (Tiles)";
            // 
            // heightLabel
            // 
            this.heightLabel.AutoSize = true;
            this.heightLabel.Location = new System.Drawing.Point(11, 41);
            this.heightLabel.Name = "heightLabel";
            this.heightLabel.Size = new System.Drawing.Size(69, 13);
            this.heightLabel.TabIndex = 2;
            this.heightLabel.Text = "Height (Tiles)";
            // 
            // heightTextBox
            // 
            this.heightTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.heightTextBox.Location = new System.Drawing.Point(86, 38);
            this.heightTextBox.Name = "heightTextBox";
            this.heightTextBox.Size = new System.Drawing.Size(53, 20);
            this.heightTextBox.TabIndex = 3;
            this.heightTextBox.Text = "64";
            this.heightTextBox.TextChanged += new System.EventHandler(this.HeightTextChanged);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(50, 90);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Create";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(131, 90);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.convertedWidthLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.convertedWidthLabel.AutoSize = true;
            this.convertedWidthLabel.Location = new System.Drawing.Point(145, 15);
            this.convertedWidthLabel.Name = "convertedWidthLabel";
            this.convertedWidthLabel.Size = new System.Drawing.Size(19, 13);
            this.convertedWidthLabel.TabIndex = 6;
            this.convertedWidthLabel.Text = "(4)";
            // 
            // label4
            // 
            this.convertedHeightLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.convertedHeightLabel.AutoSize = true;
            this.convertedHeightLabel.Location = new System.Drawing.Point(145, 41);
            this.convertedHeightLabel.Name = "convertedHeightLabel";
            this.convertedHeightLabel.Size = new System.Drawing.Size(19, 13);
            this.convertedHeightLabel.TabIndex = 7;
            this.convertedHeightLabel.Text = "(4)";
            // 
            // NewMapForm
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(218, 125);
            this.Controls.Add(this.convertedHeightLabel);
            this.Controls.Add(this.convertedWidthLabel);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.heightTextBox);
            this.Controls.Add(this.heightLabel);
            this.Controls.Add(this.widthLabel);
            this.Controls.Add(this.widthTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "NewMapForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Map";
            this.Validating += new System.ComponentModel.CancelEventHandler(this.FormValidating);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox widthTextBox;
        private System.Windows.Forms.Label widthLabel;
        private System.Windows.Forms.Label heightLabel;
        private System.Windows.Forms.TextBox heightTextBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label convertedWidthLabel;
        private System.Windows.Forms.Label convertedHeightLabel;
    }
}