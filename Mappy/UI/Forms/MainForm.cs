namespace Mappy.UI.Forms
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Forms;

    using Geometry;

    using Mappy.Models;

    public partial class MainForm : Form
    {
        private const string ProgramName = "Mappy";

        public MainForm()
        {
            this.InitializeComponent();
        }

        public IMainFormModel Model { get; private set; }

        public Rectangle ViewportRect
        {
            get
            {
                Point loc = this.mapView.AutoScrollPosition;
                loc.X *= -1;
                loc.Y *= -1;
                return new Rectangle(loc, this.mapView.ClientSize);
            }
        }

        public int SeaLevel
        {
            get
            {
                return this.seaLevelTrackbar.Value;
            }

            set
            {
                this.seaLevelTrackbar.Value = value;
                this.seaLevelValueLabel.Text = value.ToString(CultureInfo.CurrentCulture);
            }
        }

        public bool SeaLevelEditEnabled
        {
            get
            {
                return this.seaLevelTrackbar.Enabled;
            }

            set
            {
                this.seaLevelLabel.Enabled = value;
                this.seaLevelValueLabel.Enabled = value;
                this.seaLevelTrackbar.Enabled = value;
            }
        }

        public void SetModel(IMainFormModel model)
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
            this.mapView.AutoScrollPosition = new Point(x, y);
        }

        private void OpenMenuItemClick(object sender, EventArgs e)
        {
            this.Model.Open();
        }

        private void ToggleHeightmapMenuItemClick(object sender, EventArgs e)
        {
            this.Model.ToggleHeightmap();
        }

        private void MapPanel1SizeChanged(object sender, EventArgs e)
        {
            this.UpdateMinimapViewport();
        }

        private void PreferencesMenuItemClick(object sender, EventArgs e)
        {
            this.Model.OpenPreferences();
        }

        private void SaveAsMenuItemClick(object sender, EventArgs e)
        {
            this.Model.SaveAs();
        }

        private void SaveMenuItemClick(object sender, EventArgs e)
        {
            this.Model.Save();
        }

        private void ToggleMinimapMenuItemClick(object sender, EventArgs e)
        {
            this.Model.ToggleMinimap();
        }

        private void UndoMenuItemClick(object sender, EventArgs e)
        {
            this.Model.Undo();
        }

        private void RedoMenuItemClick(object sender, EventArgs e)
        {
            this.Model.Redo();
        }

        private void MainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Model.Close(); 
                e.Cancel = true;
            }
        }

        private void ExitMenuItemClick(object sender, EventArgs e)
        {
            this.Model.Close(); 
        }

        private void NewMenuItemClick(object sender, EventArgs e)
        {
            this.Model.New();
        }

        private void AboutMenuItemClick(object sender, EventArgs e)
        {
            this.Model.ShowAbout();
        }

        private void GenerateMinimapMenuItemClick(object sender, EventArgs e)
        {
            this.Model.RefreshMinimap();
        }

        private void ClearGridCheckboxes()
        {
            gridOffMenuItem.Checked = false;
            grid16MenuItem.Checked = false;
            grid32MenuItem.Checked = false;
            grid64MenuItem.Checked = false;
            grid128MenuItem.Checked = false;
            grid256MenuItem.Checked = false;
            grid512MenuItem.Checked = false;
            grid1024MenuItem.Checked = false;
        }

        private void GridOffMenuItemClick(object sender, EventArgs e)
        {
            this.Model.GridVisible = false;
        }

        private void GridMenuItemClick(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            int size = Convert.ToInt32(item.Tag);

            this.Model.GridSize = new Size(size, size);
            this.Model.GridVisible = true;
        }

        private void UpdateGridCheckboxes()
        {
            this.ClearGridCheckboxes();

            var size = this.Model.GridSize;

            if (!this.Model.GridVisible)
            {
                gridOffMenuItem.Checked = true;
                return;
            }

            if (size.Width != size.Height)
            {
                return;
            }

            switch (size.Width)
            {
                case 16:
                    this.grid16MenuItem.Checked = true;
                    break;
                case 32:
                    this.grid32MenuItem.Checked = true;
                    break;
                case 64:
                    this.grid64MenuItem.Checked = true;
                    break;
                case 128:
                    this.grid128MenuItem.Checked = true;
                    break;
                case 256:
                    this.grid256MenuItem.Checked = true;
                    break;
                case 512:
                    this.grid512MenuItem.Checked = true;
                    break;
                case 1024:
                    this.grid1024MenuItem.Checked = true;
                    break;
            }
        }

        private void GridColorMenuItemClick(object sender, EventArgs e)
        {
            this.Model.ChooseColor();
        }

        private void ToggleFeaturesMenuItemClick(object sender, EventArgs e)
        {
            this.Model.ToggleFeatures();
        }

        private void MapAttributesMenuItemClick(object sender, EventArgs e)
        {
            this.Model.OpenMapAttributes();
        }

        private void TrackBar1ValueChanged(object sender, EventArgs e)
        {
            this.Model.SetSeaLevel(this.seaLevelTrackbar.Value);
        }

        private void CloseMenuItemClick(object sender, EventArgs e)
        {
            this.Model.CloseMap();
        }

        private void GenerateMinimapHighQualityMenuItemClick(object sender, EventArgs e)
        {
            this.Model.RefreshMinimapHighQualityWithProgress();
        }

        private void SeaLevelTrackbarMouseUp(object sender, MouseEventArgs e)
        {
            this.Model.FlushSeaLevel();
        }

        private void CopyMenuItemClick(object sender, EventArgs e)
        {
            this.Model.CopySelectionToClipboard();
        }

        private void PasteMenuItemClick(object sender, EventArgs e)
        {
            this.Model.PasteFromClipboard();
        }

        private void CutMenuItemClick(object sender, EventArgs e)
        {
            this.Model.CutSelectionToClipboard();
        }

        private void MainFormLoad(object sender, EventArgs e)
        {
            this.Model.Initialize();
        }

        private void ExportMinimapMenuItemClick(object sender, EventArgs e)
        {
            this.Model.ExportMinimap();
        }

        private void ExportHeightmapMenuItemClick(object sender, EventArgs e)
        {
            this.Model.ExportHeightmap();
        }

        private void MapViewScroll(object sender, ScrollEventArgs e)
        {
            this.UpdateMinimapViewport();
        }

        private void ImportMinimapMenuItemClick(object sender, EventArgs e)
        {
            this.Model.ImportMinimap();
        }

        private void ImportHeightmapMenuItemClick(object sender, EventArgs e)
        {
            this.Model.ImportHeightmap();
        }

        private void ExportMapImageMenuItemClick(object sender, EventArgs e)
        {
            this.Model.ExportMapImage();
        }

        private void ImportCustomSectionMenuItemClick(object sender, EventArgs e)
        {
            this.Model.ImportCustomSection();
        }

        private void ModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Sections":
                    this.sectionsView.Sections = this.Model.Sections;
                    break;
                case "FeatureRecords":
                    this.featureView.Features = this.Model.FeatureRecords.EnumerateAll().ToList();
                    break;
                case "CanUndo":
                    this.undoMenuItem.Enabled = this.Model.CanUndo;
                    break;
                case "CanRedo":
                    this.redoMenuItem.Enabled = this.Model.CanRedo;
                    break;
                case "CanCopy":
                    this.copyMenuItem.Enabled = this.Model.CanCopy;
                    break;
                case "CanCut":
                    this.cutMenuItem.Enabled = this.Model.CanCut;
                    break;
                case "CanPaste":
                    this.pasteMenuItem.Enabled = this.Model.CanPaste;
                    break;
                case "MapOpen":
                    this.UpdateSave();
                    this.mapAttributesMenuItem.Enabled = this.Model.MapOpen;
                    this.UpdateMinimapViewport();
                    this.closeMenuItem.Enabled = this.Model.MapOpen;
                    this.SeaLevelEditEnabled = this.Model.MapOpen;
                    this.generateMinimapMenuItem.Enabled = this.Model.MapOpen;
                    this.generateMinimapHighQualityMenuItem.Enabled = this.Model.MapOpen;
                    this.importMinimapMenuItem.Enabled = this.Model.MapOpen;
                    this.exportMinimapMenuItem.Enabled = this.Model.MapOpen;
                    this.exportHeightmapMenuItem.Enabled = this.Model.MapOpen;
                    this.importHeightmapMenuItem.Enabled = this.Model.MapOpen;
                    this.exportMapImageMenuItem.Enabled = this.Model.MapOpen;
                    this.importCustomSectionMenuItem.Enabled = this.Model.MapOpen;
                    break;
                case "IsFileOpen":
                    this.saveAsMenuItem.Enabled = this.Model.IsFileOpen;
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
                    this.toggleMinimapMenuItem.Checked = this.Model.MinimapVisible;
                    break;
                case "HeightmapVisible":
                    this.toggleHeightmapMenuItem.Checked = this.Model.HeightmapVisible;
                    break;
                case "FeaturesVisible":
                    this.toggleFeaturesMenuItem.Checked = this.Model.FeaturesVisible;
                    break;
                case "ViewportRectangle":
                    this.UpdateViewViewportRect();
                    break;
                case "GridVisible":
                case "GridSize":
                    this.UpdateGridCheckboxes();
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
            this.saveMenuItem.Enabled = this.Model.MapOpen && this.Model.FilePath != null && !this.Model.IsFileReadOnly;
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
