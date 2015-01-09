namespace Mappy.UI.Forms
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Forms;

    using Geometry;

    using Mappy.Data;
    using Mappy.Models;

    public partial class MainForm : Form
    {
        private const string ProgramName = "Mappy";

        private IList<Section> sections;
        private IList<Feature> features;

        public MainForm()
        {
            this.InitializeComponent();
        }

        public IMainModel Model { get; private set; }

        public Rectangle ViewportRect
        {
            get
            {
                Point loc = this.imageLayerView1.AutoScrollPosition;
                loc.X *= -1;
                loc.Y *= -1;
                return new Rectangle(loc, this.imageLayerView1.ClientSize);
            }
        }

        public IList<Section> Sections
        {
            get
            {
                return this.sections;
            }

            set
            {
                this.sections = value;
                this.sectionView1.Sections = value;
            }
        }

        public IList<Feature> Features
        {
            get
            {
                return this.features;
            }

            set
            {
                this.features = value;
                this.featureview1.Features = value;
            }
        }

        public int SeaLevel
        {
            get
            {
                return this.trackBar1.Value;
            }

            set
            {
                this.trackBar1.Value = value;
                this.label3.Text = value.ToString(CultureInfo.CurrentCulture);
            }
        }

        public bool SeaLevelEditEnabled
        {
            get
            {
                return this.trackBar1.Enabled;
            }

            set
            {
                this.label2.Enabled = value;
                this.label3.Enabled = value;
                this.trackBar1.Enabled = value;
            }
        }

        public void SetModel(IMainModel model)
        {
            model.PropertyChanged += this.ModelOnPropertyChanged;
            this.Model = model;
        }

        public void UpdateMinimapViewport()
        {
            if (this.Model == null)
            {
                return;
            }

            this.Model.ViewportRectangle = this.ConvertToNormalizedViewport(this.ViewportRect);
        }

        public void SetViewportPosition(int x, int y)
        {
            this.imageLayerView1.AutoScrollPosition = new Point(x, y);
        }

        private void OpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Model.Open();
        }

        private void HeightmapToolStripMenuItemCheckedChanged(object sender, EventArgs e)
        {
            this.Model.ToggleHeightmap();
        }

        private void MapPanel1SizeChanged(object sender, EventArgs e)
        {
            this.UpdateMinimapViewport();
        }

        private void PreferencesToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Model.OpenPreferences();
        }

        private void ToolStripMenuItem4Click(object sender, EventArgs e)
        {
            this.Model.SaveAs();
        }

        private void ToolStripMenuItem5Click(object sender, EventArgs e)
        {
            this.Model.Save();
        }

        private void MinimapToolStripMenuItem1Click(object sender, EventArgs e)
        {
            this.Model.ToggleMinimap();
        }

        private void UndoToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Model.Undo();
        }

        private void RedoToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Model.Redo();
        }

        private void Form1FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Model.Close(); 
                e.Cancel = true;
            }
        }

        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Model.Close(); 
        }

        private void ToolStripMenuItem2Click(object sender, EventArgs e)
        {
            this.Model.New();
        }

        private void AboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Model.ShowAbout();
        }

        private void ToolStripMenuItem6Click(object sender, EventArgs e)
        {
            this.Model.RefreshMinimap();
        }

        private void ClearGridCheckboxes()
        {
            offToolStripMenuItem.Checked = false;
            toolStripMenuItem10.Checked = false;
            toolStripMenuItem9.Checked = false;
            toolStripMenuItem8.Checked = false;
            toolStripMenuItem7.Checked = false;
            x256ToolStripMenuItem.Checked = false;
            x512ToolStripMenuItem.Checked = false;
            x1024ToolStripMenuItem.Checked = false;
        }

        private void OffToolStripMenuItemClick(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            this.ClearGridCheckboxes();
            item.Checked = true;
            int size = Convert.ToInt32(item.Tag);

            this.Model.SetGridSize(size);
        }

        private void ChooseColorToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Model.ChooseColor();
        }

        private void FeaturesToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Model.ToggleFeatures();
        }

        private void ToolStripMenuItem11Click(object sender, EventArgs e)
        {
            this.Model.OpenMapAttributes();
        }

        private void TrackBar1ValueChanged(object sender, EventArgs e)
        {
            this.Model.SetSeaLevel(this.trackBar1.Value);
        }

        private void toolStripMenuItem12_Click(object sender, EventArgs e)
        {
            this.Model.CloseMap();
        }

        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {
            this.Model.RefreshMinimapHighQualityWithProgress();
        }

        private void trackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            this.Model.FlushSeaLevel();
        }

        private void toolStripMenuItem14_Click(object sender, EventArgs e)
        {
            this.Model.CopySelectionToClipboard();
        }

        private void toolStripMenuItem15_Click(object sender, EventArgs e)
        {
            this.Model.PasteFromClipboard();
        }

        private void toolStripMenuItem16_Click(object sender, EventArgs e)
        {
            this.Model.CutSelectionToClipboard();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Model.Initialize();
        }

        private void toolStripMenuItem17_Click(object sender, EventArgs e)
        {
            this.Model.ExportMinimap();
        }

        private void toolStripMenuItem18_Click(object sender, EventArgs e)
        {
            this.Model.ExportHeightmap();
        }

        private void imageLayerView1_Scroll(object sender, ScrollEventArgs e)
        {
            this.UpdateMinimapViewport();
        }

        private void toolStripMenuItem19_Click(object sender, EventArgs e)
        {
            this.Model.ImportMinimap();
        }

        private void toolStripMenuItem20_Click(object sender, EventArgs e)
        {
            this.Model.ImportHeightmap();
        }

        private void toolStripMenuItem21_Click(object sender, EventArgs e)
        {
            this.Model.ExportMapImage();
        }

        private void toolStripMenuItem22_Click(object sender, EventArgs e)
        {
            this.Model.ImportCustomSection();
        }

        private void ModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Sections":
                    this.Sections = this.Model.Sections;
                    break;
                case "FeatureRecords":
                    this.Features = this.Model.FeatureRecords.EnumerateAll().ToList();
                    break;
                case "CanUndo":
                    this.undoToolStripMenuItem.Enabled = this.Model.CanUndo;
                    break;
                case "CanRedo":
                    this.redoToolStripMenuItem.Enabled = this.Model.CanRedo;
                    break;
                case "CanCopy":
                    this.toolStripMenuItem14.Enabled = this.Model.CanCopy;
                    break;
                case "CanCut":
                    this.toolStripMenuItem16.Enabled = this.Model.CanCut;
                    break;
                case "CanPaste":
                    this.toolStripMenuItem15.Enabled = this.Model.CanPaste;
                    break;
                case "MapOpen":
                    this.UpdateSave();
                    this.toolStripMenuItem11.Enabled = this.Model.MapOpen;
                    this.UpdateMinimapViewport();
                    this.toolStripMenuItem12.Enabled = this.Model.MapOpen;
                    this.SeaLevelEditEnabled = this.Model.MapOpen;
                    this.toolStripMenuItem6.Enabled = this.Model.MapOpen;
                    this.toolStripMenuItem13.Enabled = this.Model.MapOpen;
                    this.toolStripMenuItem19.Enabled = this.Model.MapOpen;
                    this.toolStripMenuItem17.Enabled = this.Model.MapOpen;
                    this.toolStripMenuItem18.Enabled = this.Model.MapOpen;
                    this.toolStripMenuItem20.Enabled = this.Model.MapOpen;
                    this.toolStripMenuItem21.Enabled = this.Model.MapOpen;
                    this.toolStripMenuItem22.Enabled = this.Model.MapOpen;
                    break;
                case "IsFileOpen":
                    this.toolStripMenuItem4.Enabled = this.Model.IsFileOpen;
                    this.UpdateTitleText();
                    break;
                case "FilePath":
                    this.UpdateSave();
                    this.UpdateTitleText();
                    break;
                case "IsDirty":
                    this.UpdateTitleText();
                    break;
                case "IsFileReadOnly":
                    this.UpdateSave();
                    this.UpdateTitleText();
                    break;
                case "SeaLevel":
                    this.SeaLevel = this.Model.SeaLevel;
                    break;
                case "MinimapVisible":
                    this.minimapToolStripMenuItem1.Checked = this.Model.MinimapVisible;
                    break;
                case "ViewportRectangle":
                    this.UpdateViewViewportRect();
                    break;
            }
        }

        private Rectangle2D ConvertToNormalizedViewport(Rectangle rect)
        {
            if (!this.Model.MapOpen)
            {
                return Rectangle2D.Empty;
            }

            int widthScale = (this.Model.MapWidth * 32) - 32;
            int heightScale = (this.Model.MapHeight * 32) - 128;

            double x = rect.X / (double)widthScale;
            double y = rect.Y / (double)heightScale;
            double w = rect.Width / (double)widthScale;
            double h = rect.Height / (double)heightScale;

            return Rectangle2D.FromCorner(x, y, w, h);
        }

        private Rectangle ConvertToClientViewport(Rectangle2D rect)
        {
            if (!this.Model.MapOpen)
            {
                return Rectangle.Empty;
            }

            int widthScale = (this.Model.MapWidth * 32) - 32;
            int heightScale = (this.Model.MapHeight * 32) - 128;

            int x = (int)Math.Round(rect.MinX * widthScale);
            int y = (int)Math.Round(rect.MinY * heightScale);
            int w = (int)Math.Round(rect.Width * widthScale);
            int h = (int)Math.Round(rect.Height * heightScale);

            return new Rectangle(x, y, w, h);
        }

        private void UpdateViewViewportRect()
        {
            var rect = this.Model.ViewportRectangle;
            var clientRect = this.ConvertToClientViewport(rect);
            this.SetViewportPosition(clientRect.X, clientRect.Y);
        }

        private void UpdateSave()
        {
            this.toolStripMenuItem5.Enabled = this.Model.MapOpen && this.Model.FilePath != null && !this.Model.IsFileReadOnly;
        }

        private void UpdateTitleText()
        {
            this.Text = this.GenerateTitleText();
        }

        private string GenerateTitleText()
        {
            if (!this.Model.IsFileOpen)
            {
                return ProgramName;
            }

            string filename = this.Model.FilePath ?? "Untitled";
            if (this.Model.IsDirty)
            {
                filename += "*";
            }

            if (this.Model.IsFileReadOnly)
            {
                filename += " [read only]";
            }

            return filename + " - " + ProgramName;
        }
    }
}
