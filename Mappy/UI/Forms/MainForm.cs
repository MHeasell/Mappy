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

        private IMainFormModel model;

        public MainForm()
        {
            this.InitializeComponent();
        }

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

        public void SetModel(IMainFormModel model)
        {
            model.PropertyChanged += this.ModelOnPropertyChanged;
            this.model = model;
        }

        public void UpdateMinimapViewport()
        {
            if (this.model == null)
            {
                return;
            }

            this.model.ViewportRectangle = this.ConvertToNormalizedViewport(this.ViewportRect);
        }

        public void SetViewportPosition(int x, int y)
        {
            this.mapView.AutoScrollPosition = new Point(x, y);
        }

        private void OpenMenuItemClick(object sender, EventArgs e)
        {
            this.model.Open();
        }

        private void ToggleHeightmapMenuItemClick(object sender, EventArgs e)
        {
            this.model.ToggleHeightmap();
        }

        private void MapPanel1SizeChanged(object sender, EventArgs e)
        {
            this.UpdateMinimapViewport();
        }

        private void PreferencesMenuItemClick(object sender, EventArgs e)
        {
            this.model.OpenPreferences();
        }

        private void SaveAsMenuItemClick(object sender, EventArgs e)
        {
            this.model.SaveAs();
        }

        private void SaveMenuItemClick(object sender, EventArgs e)
        {
            this.model.Save();
        }

        private void ToggleMinimapMenuItemClick(object sender, EventArgs e)
        {
            this.model.ToggleMinimap();
        }

        private void UndoMenuItemClick(object sender, EventArgs e)
        {
            this.model.Undo();
        }

        private void RedoMenuItemClick(object sender, EventArgs e)
        {
            this.model.Redo();
        }

        private void MainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.model.Close(); 
                e.Cancel = true;
            }
        }

        private void ExitMenuItemClick(object sender, EventArgs e)
        {
            this.model.Close(); 
        }

        private void NewMenuItemClick(object sender, EventArgs e)
        {
            this.model.New();
        }

        private void AboutMenuItemClick(object sender, EventArgs e)
        {
            this.model.ShowAbout();
        }

        private void GenerateMinimapMenuItemClick(object sender, EventArgs e)
        {
            this.model.RefreshMinimap();
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
            this.model.GridVisible = false;
        }

        private void GridMenuItemClick(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            int size = Convert.ToInt32(item.Tag);

            this.model.GridSize = new Size(size, size);
            this.model.GridVisible = true;
        }

        private void UpdateGridCheckboxes()
        {
            this.ClearGridCheckboxes();

            var size = this.model.GridSize;

            if (!this.model.GridVisible)
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
            this.model.ChooseColor();
        }

        private void ToggleFeaturesMenuItemClick(object sender, EventArgs e)
        {
            this.model.ToggleFeatures();
        }

        private void MapAttributesMenuItemClick(object sender, EventArgs e)
        {
            this.model.OpenMapAttributes();
        }

        private void TrackBar1ValueChanged(object sender, EventArgs e)
        {
            this.model.SetSeaLevel(this.seaLevelTrackbar.Value);
        }

        private void CloseMenuItemClick(object sender, EventArgs e)
        {
            this.model.CloseMap();
        }

        private void GenerateMinimapHighQualityMenuItemClick(object sender, EventArgs e)
        {
            this.model.RefreshMinimapHighQualityWithProgress();
        }

        private void SeaLevelTrackbarMouseUp(object sender, MouseEventArgs e)
        {
            this.model.FlushSeaLevel();
        }

        private void CopyMenuItemClick(object sender, EventArgs e)
        {
            this.model.CopySelectionToClipboard();
        }

        private void PasteMenuItemClick(object sender, EventArgs e)
        {
            this.model.PasteFromClipboard();
        }

        private void CutMenuItemClick(object sender, EventArgs e)
        {
            this.model.CutSelectionToClipboard();
        }

        private void MainFormLoad(object sender, EventArgs e)
        {
            this.model.Initialize();
        }

        private void ExportMinimapMenuItemClick(object sender, EventArgs e)
        {
            this.model.ExportMinimap();
        }

        private void ExportHeightmapMenuItemClick(object sender, EventArgs e)
        {
            this.model.ExportHeightmap();
        }

        private void MapViewScroll(object sender, ScrollEventArgs e)
        {
            this.UpdateMinimapViewport();
        }

        private void ImportMinimapMenuItemClick(object sender, EventArgs e)
        {
            this.model.ImportMinimap();
        }

        private void ImportHeightmapMenuItemClick(object sender, EventArgs e)
        {
            this.model.ImportHeightmap();
        }

        private void ExportMapImageMenuItemClick(object sender, EventArgs e)
        {
            this.model.ExportMapImage();
        }

        private void ImportCustomSectionMenuItemClick(object sender, EventArgs e)
        {
            this.model.ImportCustomSection();
        }

        private void ModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Sections":
                    this.sectionsView.Sections = this.model.Sections;
                    break;
                case "FeatureRecords":
                    this.featureView.Features = this.model.FeatureRecords.EnumerateAll().ToList();
                    break;
                case "CanUndo":
                    this.undoMenuItem.Enabled = this.model.CanUndo;
                    break;
                case "CanRedo":
                    this.redoMenuItem.Enabled = this.model.CanRedo;
                    break;
                case "CanCopy":
                    this.copyMenuItem.Enabled = this.model.CanCopy;
                    break;
                case "CanCut":
                    this.cutMenuItem.Enabled = this.model.CanCut;
                    break;
                case "CanPaste":
                    this.pasteMenuItem.Enabled = this.model.CanPaste;
                    break;
                case "MapOpen":
                    this.UpdateSave();
                    this.mapAttributesMenuItem.Enabled = this.model.MapOpen;
                    this.UpdateMinimapViewport();
                    this.closeMenuItem.Enabled = this.model.MapOpen;
                    this.seaLevelLabel.Enabled = this.model.MapOpen;
                    this.seaLevelValueLabel.Enabled = this.model.MapOpen;
                    this.seaLevelTrackbar.Enabled = this.model.MapOpen;
                    this.generateMinimapMenuItem.Enabled = this.model.MapOpen;
                    this.generateMinimapHighQualityMenuItem.Enabled = this.model.MapOpen;
                    this.importMinimapMenuItem.Enabled = this.model.MapOpen;
                    this.exportMinimapMenuItem.Enabled = this.model.MapOpen;
                    this.exportHeightmapMenuItem.Enabled = this.model.MapOpen;
                    this.importHeightmapMenuItem.Enabled = this.model.MapOpen;
                    this.exportMapImageMenuItem.Enabled = this.model.MapOpen;
                    this.importCustomSectionMenuItem.Enabled = this.model.MapOpen;
                    break;
                case "IsFileOpen":
                    this.saveAsMenuItem.Enabled = this.model.IsFileOpen;
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
                    int val = this.model.SeaLevel;
                    this.seaLevelTrackbar.Value = val;
                    this.seaLevelValueLabel.Text = val.ToString(CultureInfo.CurrentCulture);
                    break;
                case "MinimapVisible":
                    this.toggleMinimapMenuItem.Checked = this.model.MinimapVisible;
                    break;
                case "HeightmapVisible":
                    this.toggleHeightmapMenuItem.Checked = this.model.HeightmapVisible;
                    break;
                case "FeaturesVisible":
                    this.toggleFeaturesMenuItem.Checked = this.model.FeaturesVisible;
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
            if (!this.model.MapOpen)
            {
                return Rectangle2D.Empty;
            }

            int widthScale = (this.model.MapWidth * 32) - 32;
            int heightScale = (this.model.MapHeight * 32) - 128;

            double x = rect.X / (double)widthScale;
            double y = rect.Y / (double)heightScale;
            double w = rect.Width / (double)widthScale;
            double h = rect.Height / (double)heightScale;

            return Rectangle2D.FromCorner(x, y, w, h);
        }

        private Rectangle ConvertToClientViewport(Rectangle2D rect)
        {
            if (!this.model.MapOpen)
            {
                return Rectangle.Empty;
            }

            int widthScale = (this.model.MapWidth * 32) - 32;
            int heightScale = (this.model.MapHeight * 32) - 128;

            int x = (int)Math.Round(rect.MinX * widthScale);
            int y = (int)Math.Round(rect.MinY * heightScale);
            int w = (int)Math.Round(rect.Width * widthScale);
            int h = (int)Math.Round(rect.Height * heightScale);

            return new Rectangle(x, y, w, h);
        }

        private void UpdateViewViewportRect()
        {
            var rect = this.model.ViewportRectangle;
            var clientRect = this.ConvertToClientViewport(rect);
            this.SetViewportPosition(clientRect.X, clientRect.Y);
        }

        private void UpdateSave()
        {
            this.saveMenuItem.Enabled = this.model.MapOpen && this.model.FilePath != null && !this.model.IsFileReadOnly;
        }

        private void UpdateTitleText()
        {
            this.Text = this.GenerateTitleText();
        }

        private string GenerateTitleText()
        {
            if (!this.model.IsFileOpen)
            {
                return ProgramName;
            }

            string filename = this.model.FilePath ?? "Untitled";
            if (this.model.IsDirty)
            {
                filename += "*";
            }

            if (this.model.IsFileReadOnly)
            {
                filename += " [read only]";
            }

            return filename + " - " + ProgramName;
        }
    }
}
