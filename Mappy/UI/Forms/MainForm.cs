namespace Mappy.UI.Forms
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Windows.Forms;

    using Mappy.Models;
    using Mappy.UI.Controls;

    public partial class MainForm : Form
    {
        private const string ProgramName = "Mappy";

        private IUserEventDispatcher dispatcher;

        public MainForm()
        {
            this.InitializeComponent();
        }

        public MapViewPanel MapViewPanel => this.mapViewPanel;

        public void SetModel(IMainFormModel model)
        {
            var gridSizeStream = model.GridSize.CombineLatest(
                model.GridVisible,
                (size, visible) => visible ? new Size?(size) : null);

            gridSizeStream.Subscribe(this.UpdateGridSize);

            model.CanUndo.Subscribe(x => this.undoMenuItem.Enabled = x);
            model.CanRedo.Subscribe(x => this.redoMenuItem.Enabled = x);
            model.CanCopy.Subscribe(x => this.copyMenuItem.Enabled = x);
            model.CanCut.Subscribe(x => this.cutMenuItem.Enabled = x);
            model.CanPaste.Subscribe(x => this.pasteMenuItem.Enabled = x);

            // map open bindings
            model.MapOpen.Subscribe(x => this.mapAttributesMenuItem.Enabled = x);
            model.MapOpen.Subscribe(x => this.closeMenuItem.Enabled = x);

            model.MapOpen.Subscribe(x => this.seaLevelLabel.Enabled = x);
            model.MapOpen.Subscribe(x => this.seaLevelValueLabel.Enabled = x);
            model.MapOpen.Subscribe(x => this.seaLevelTrackbar.Enabled = x);

            model.MapOpen.Subscribe(x => this.generateMinimapMenuItem.Enabled = x);
            model.MapOpen.Subscribe(x => this.generateMinimapHighQualityMenuItem.Enabled = x);

            model.MapOpen.Subscribe(x => this.importMinimapMenuItem.Enabled = x);
            model.MapOpen.Subscribe(x => this.exportMinimapMenuItem.Enabled = x);

            model.MapOpen.Subscribe(x => this.importHeightmapMenuItem.Enabled = x);
            model.MapOpen.Subscribe(x => this.exportHeightmapMenuItem.Enabled = x);

            model.MapOpen.Subscribe(x => this.exportMapImageMenuItem.Enabled = x);
            model.MapOpen.Subscribe(x => this.importCustomSectionMenuItem.Enabled = x);

            model.MapOpen.Subscribe(x => this.saveAsMenuItem.Enabled = x);

            // save menu item logic
            Observable.CombineLatest(
                model.MapOpen,
                model.FilePath.Select(x => x != null),
                model.IsFileReadOnly.Select(x => !x))
                .Select(x => x.All(y => y))
                .Subscribe(x => this.saveMenuItem.Enabled = x);

            // window title logic
            model.FilePath.Subscribe(x => Console.WriteLine("" + x));
            var cleanFilenameStream = model.FilePath.Select(x => (x ?? "Untitled"));
            var dirtyFilenameStream = cleanFilenameStream.Select(x => x + "*");

            var filenameStream = model.IsDirty
                .Select(x => x ? dirtyFilenameStream : cleanFilenameStream)
                .Switch();
            var readOnlyFilenameStream = filenameStream.Select(y => y + " [read only]");

            var filenameTitleStream = model.IsFileReadOnly
                .Select(x => x ? readOnlyFilenameStream : filenameStream)
                .Switch();

            var programNameStream = Observable.Return(ProgramName);
            var openFileTitleStream = filenameTitleStream.Select(y => y + " - " + ProgramName);
            var titleStream = model.MapOpen
                .Select(x => x ? openFileTitleStream : programNameStream)
                .Switch();

            titleStream.Subscribe(x => this.Text = x);

            // sea level
            model.SeaLevel.Subscribe(x => this.seaLevelTrackbar.Value = x);
            model.SeaLevel
                .Select(x => x.ToString(CultureInfo.CurrentCulture))
                .Subscribe(x => this.seaLevelValueLabel.Text = x);

            // minimap
            model.MinimapVisible.Subscribe(x => this.toggleMinimapMenuItem.Checked = x);

            // heightmap
            model.HeightmapVisible.Subscribe(x => this.toggleHeightmapMenuItem.Checked = x);

            // features
            model.FeaturesVisible.Subscribe(x => this.toggleFeaturesMenuItem.Checked = x);

            // sections
            model.Sections.Subscribe(x => this.sectionsView.Sections = x);

            // feature records
            model.FeatureRecords.Subscribe(x => this.featureView.Features = x.EnumerateAll().ToList());
        }

        private void UpdateGridSize(Size? sizeContainer)
        {
            this.ClearGridCheckboxes();

            if (!sizeContainer.HasValue)
            {
                this.gridOffMenuItem.Checked = true;
                return;
            }

            var size = sizeContainer.Value;

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

        public void SetDispatcher(IUserEventDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        private void OpenMenuItemClick(object sender, EventArgs e)
        {
            this.dispatcher.Open();
        }

        private void ToggleHeightmapMenuItemClick(object sender, EventArgs e)
        {
            this.dispatcher.ToggleHeightmap();
        }

        private void PreferencesMenuItemClick(object sender, EventArgs e)
        {
            this.dispatcher.OpenPreferences();
        }

        private void SaveAsMenuItemClick(object sender, EventArgs e)
        {
            this.dispatcher.SaveAs();
        }

        private void SaveMenuItemClick(object sender, EventArgs e)
        {
            this.dispatcher.Save();
        }

        private void ToggleMinimapMenuItemClick(object sender, EventArgs e)
        {
            this.dispatcher.ToggleMinimap();
        }

        private void UndoMenuItemClick(object sender, EventArgs e)
        {
            this.dispatcher.Undo();
        }

        private void RedoMenuItemClick(object sender, EventArgs e)
        {
            this.dispatcher.Redo();
        }

        private void MainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.dispatcher.Close();
                e.Cancel = true;
            }
        }

        private void ExitMenuItemClick(object sender, EventArgs e)
        {
            this.dispatcher.Close();
        }

        private void NewMenuItemClick(object sender, EventArgs e)
        {
            this.dispatcher.New();
        }

        private void AboutMenuItemClick(object sender, EventArgs e)
        {
            this.dispatcher.ShowAbout();
        }

        private void GenerateMinimapMenuItemClick(object sender, EventArgs e)
        {
            this.dispatcher.RefreshMinimap();
        }

        private void ClearGridCheckboxes()
        {
            this.gridOffMenuItem.Checked = false;
            this.grid16MenuItem.Checked = false;
            this.grid32MenuItem.Checked = false;
            this.grid64MenuItem.Checked = false;
            this.grid128MenuItem.Checked = false;
            this.grid256MenuItem.Checked = false;
            this.grid512MenuItem.Checked = false;
            this.grid1024MenuItem.Checked = false;
        }

        private void GridOffMenuItemClick(object sender, EventArgs e)
        {
            this.dispatcher.HideGrid();
        }

        private void GridMenuItemClick(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            int size = Convert.ToInt32(item.Tag);
            Size s = new Size(size, size);

            this.dispatcher.EnableGridWithSize(s);
        }

        private void GridColorMenuItemClick(object sender, EventArgs e)
        {
            this.dispatcher.ChooseColor();
        }

        private void ToggleFeaturesMenuItemClick(object sender, EventArgs e)
        {
            this.dispatcher.ToggleFeatures();
        }

        private void MapAttributesMenuItemClick(object sender, EventArgs e)
        {
            this.dispatcher.OpenMapAttributes();
        }

        private void TrackBar1ValueChanged(object sender, EventArgs e)
        {
            this.dispatcher.SetSeaLevel(this.seaLevelTrackbar.Value);
        }

        private void CloseMenuItemClick(object sender, EventArgs e)
        {
            this.dispatcher.CloseMap();
        }

        private void GenerateMinimapHighQualityMenuItemClick(object sender, EventArgs e)
        {
            this.dispatcher.RefreshMinimapHighQualityWithProgress();
        }

        private void SeaLevelTrackbarMouseUp(object sender, MouseEventArgs e)
        {
            this.dispatcher.FlushSeaLevel();
        }

        private void CopyMenuItemClick(object sender, EventArgs e)
        {
            this.dispatcher.CopySelectionToClipboard();
        }

        private void PasteMenuItemClick(object sender, EventArgs e)
        {
            this.dispatcher.PasteFromClipboard();
        }

        private void CutMenuItemClick(object sender, EventArgs e)
        {
            this.dispatcher.CutSelectionToClipboard();
        }

        private void MainFormLoad(object sender, EventArgs e)
        {
            this.dispatcher.Initialize();
        }

        private void ExportMinimapMenuItemClick(object sender, EventArgs e)
        {
            this.dispatcher.ExportMinimap();
        }

        private void ExportHeightmapMenuItemClick(object sender, EventArgs e)
        {
            this.dispatcher.ExportHeightmap();
        }

        private void ImportMinimapMenuItemClick(object sender, EventArgs e)
        {
            this.dispatcher.ImportMinimap();
        }

        private void ImportHeightmapMenuItemClick(object sender, EventArgs e)
        {
            this.dispatcher.ImportHeightmap();
        }

        private void ExportMapImageMenuItemClick(object sender, EventArgs e)
        {
            this.dispatcher.ExportMapImage();
        }

        private void ImportCustomSectionMenuItemClick(object sender, EventArgs e)
        {
            this.dispatcher.ImportCustomSection();
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

                this.dispatcher.OpenFromDragDrop(data[0]);
            }
        }
    }
}
