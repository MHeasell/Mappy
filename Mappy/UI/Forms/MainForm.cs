namespace Mappy.UI.Forms
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Reactive.Linq;
    using System.Windows.Forms;

    using Mappy.Models;
    using Mappy.UI.Controls;

    public partial class MainForm : Form
    {
        private IMainFormViewModel model;

        public MainForm()
        {
            this.InitializeComponent();
        }

        public MapViewPanel MapViewPanel => this.mapViewPanel;

        public SectionView SectionView => this.sectionsView;

        public SectionView FeatureView => this.featureView;

        public void SetModel(IMainFormViewModel model)
        {
            var gridSize = model.GridSize.CombineLatest(
                model.GridVisible,
                (size, visible) => visible ? new Size?(size) : null);

            gridSize.Subscribe(x => this.gridOffMenuItem.Checked = x == null);
            gridSize.Subscribe(x => this.grid16MenuItem.Checked = x == new Size(16, 16));
            gridSize.Subscribe(x => this.grid32MenuItem.Checked = x == new Size(32, 32));
            gridSize.Subscribe(x => this.grid64MenuItem.Checked = x == new Size(64, 64));
            gridSize.Subscribe(x => this.grid128MenuItem.Checked = x == new Size(128, 128));
            gridSize.Subscribe(x => this.grid256MenuItem.Checked = x == new Size(256, 256));
            gridSize.Subscribe(x => this.grid512MenuItem.Checked = x == new Size(512, 512));
            gridSize.Subscribe(x => this.grid1024MenuItem.Checked = x == new Size(1024, 1024));

            // file menu bindings
            model.CanSave.Subscribe(x => this.saveMenuItem.Enabled = x);
            model.CanSaveAs.Subscribe(x => this.saveAsMenuItem.Enabled = x);
            model.CanCloseMap.Subscribe(x => this.closeMenuItem.Enabled = x);

            model.CanImportMinimap.Subscribe(x => this.importMinimapMenuItem.Enabled = x);
            model.CanExportMinimap.Subscribe(x => this.exportMinimapMenuItem.Enabled = x);

            model.CanImportHeightmap.Subscribe(x => this.importHeightmapMenuItem.Enabled = x);
            model.CanExportHeightmap.Subscribe(x => this.exportHeightmapMenuItem.Enabled = x);

            model.CanExportMapImage.Subscribe(x => this.exportMapImageMenuItem.Enabled = x);
            model.CanImportCustomSection.Subscribe(x => this.importCustomSectionMenuItem.Enabled = x);

            // edit menu bindings
            model.CanUndo.Subscribe(x => this.undoMenuItem.Enabled = x);
            model.CanRedo.Subscribe(x => this.redoMenuItem.Enabled = x);
            model.CanCopy.Subscribe(x => this.copyMenuItem.Enabled = x);
            model.CanCut.Subscribe(x => this.cutMenuItem.Enabled = x);
            model.CanPaste.Subscribe(x => this.pasteMenuItem.Enabled = x);

            model.CanGenerateMinimap.Subscribe(x => this.generateMinimapMenuItem.Enabled = x);
            model.CanGenerateMinimapHighQuality.Subscribe(x => this.generateMinimapHighQualityMenuItem.Enabled = x);

            model.CanOpenAttributes.Subscribe(x => this.mapAttributesMenuItem.Enabled = x);

            // view menu bindings
            model.MinimapVisible.Subscribe(x => this.toggleMinimapMenuItem.Checked = x);
            model.HeightmapVisible.Subscribe(x => this.toggleHeightmapMenuItem.Checked = x);
            model.FeaturesVisible.Subscribe(x => this.toggleFeaturesMenuItem.Checked = x);

            // sea level widget bindings
            model.CanChangeSeaLevel.Subscribe(x => this.seaLevelLabel.Enabled = x);
            model.CanChangeSeaLevel.Subscribe(x => this.seaLevelValueLabel.Enabled = x);
            model.CanChangeSeaLevel.Subscribe(x => this.seaLevelTrackbar.Enabled = x);

            model.SeaLevel.Subscribe(x => this.seaLevelTrackbar.Value = x);
            model.SeaLevel
                .Select(x => x.ToString(CultureInfo.CurrentCulture))
                .Subscribe(x => this.seaLevelValueLabel.Text = x);

            // title text bindings
            model.TitleText.Subscribe(x => this.Text = x);

            this.model = model;
        }

        private void OpenMenuItemClick(object sender, EventArgs e)
        {
            this.model.Open();
        }

        private void ToggleHeightmapMenuItemClick(object sender, EventArgs e)
        {
            this.model.ToggleHeightmap();
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

        private void GridOffMenuItemClick(object sender, EventArgs e)
        {
            this.model.HideGrid();
        }

        private void GridMenuItemClick(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            int size = Convert.ToInt32(item.Tag);
            Size s = new Size(size, size);

            this.model.EnableGridWithSize(s);
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

        private void MainFormDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void MainFormDragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var data = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (data.Length > 1)
                {
                    return;
                }

                this.model.OpenFromDragDrop(data[0]);
            }
        }
    }
}
