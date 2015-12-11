namespace Mappy.UI.Controls
{
    partial class MapViewPanel
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
            this.mapView = new Mappy.UI.Controls.ImageLayerView();
            this.SuspendLayout();
            // 
            // mapView
            // 
            this.mapView.AllowDrop = true;
            this.mapView.CanvasColor = System.Drawing.Color.CornflowerBlue;
            this.mapView.CanvasSize = new System.Drawing.Size(0, 0);
            this.mapView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapView.GridColor = System.Drawing.Color.Black;
            this.mapView.GridSize = new System.Drawing.Size(16, 16);
            this.mapView.GridVisible = false;
            this.mapView.Location = new System.Drawing.Point(0, 0);
            this.mapView.Name = "mapView";
            this.mapView.Size = new System.Drawing.Size(150, 150);
            this.mapView.TabIndex = 0;
            this.mapView.Text = "imageLayerView1";
            this.mapView.SizeChanged += new System.EventHandler(this.MapViewSizeChanged);
            this.mapView.DragDrop += new System.Windows.Forms.DragEventHandler(this.MapViewDragDrop);
            this.mapView.DragEnter += new System.Windows.Forms.DragEventHandler(this.MapViewDragEnter);
            this.mapView.Paint += new System.Windows.Forms.PaintEventHandler(this.MapViewPaint);
            this.mapView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MapViewKeyDown);
            this.mapView.Leave += new System.EventHandler(this.MapViewLeave);
            this.mapView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MapViewMouseDown);
            this.mapView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MapViewMouseMove);
            this.mapView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MapViewMouseUp);
            // 
            // MapViewPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mapView);
            this.Name = "MapViewPanel";
            this.ResumeLayout(false);

        }

        #endregion

        private ImageLayerView mapView;
    }
}
