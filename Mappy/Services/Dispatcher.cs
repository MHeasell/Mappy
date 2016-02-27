namespace Mappy.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    using Mappy.Collections;
    using Mappy.Data;
    using Mappy.IO;
    using Mappy.Models;

    using TAUtil;
    using TAUtil.Hpi;
    using TAUtil.Sct;
    using TAUtil.Tdf;
    using TAUtil.Tnt;

    public class Dispatcher
    {
        private readonly CoreModel model;

        private readonly IDialogService dialogService;

        private readonly SectionsService sectionsService;

        private readonly FeatureService featureService;

        private readonly SectionFactory sectionFactory;

        private readonly MapModelFactory mapModelFactory;

        public Dispatcher(
            CoreModel model,
            IDialogService dialogService,
            SectionsService sectionsService,
            FeatureService featureService)
        {
            this.model = model;
            this.dialogService = dialogService;
            this.sectionsService = sectionsService;
            this.featureService = featureService;

            this.sectionFactory = new SectionFactory();
            this.mapModelFactory = new MapModelFactory();
        }

        public void Initialize()
        {
            var dlg = this.dialogService.CreateProgressView();
            dlg.Title = "Loading Mappy";
            dlg.ShowProgress = true;
            dlg.CancelEnabled = true;

            var worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += delegate(object sender, DoWorkEventArgs args)
            {
                var w = (BackgroundWorker)sender;

                LoadResult<Section> result;
                if (!SectionLoadingUtils.LoadSections(
                        i => w.ReportProgress((50 * i) / 100),
                        () => w.CancellationPending,
                        out result))
                {
                    args.Cancel = true;
                    return;
                }

                LoadResult<Feature> featureResult;
                if (!FeatureLoadingUtils.LoadFeatures(
                    i => w.ReportProgress(50 + ((50 * i) / 100)),
                    () => w.CancellationPending,
                    out featureResult))
                {
                    args.Cancel = true;
                    return;
                }

                args.Result = new SectionFeatureLoadResult
                {
                    Sections = result.Records,
                    Features = featureResult.Records,
                    Errors = result.Errors
                        .Concat(featureResult.Errors)
                        .GroupBy(x => x.HpiPath)
                        .Select(x => x.First())
                        .ToList(),
                    FileErrors = result.FileErrors
                        .Concat(featureResult.FileErrors)
                        .ToList(),
                };
            };

            worker.ProgressChanged += (sender, args) => dlg.Progress = args.ProgressPercentage;
            worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs args)
            {
                if (args.Error != null)
                {
                    Program.HandleUnexpectedException(args.Error);
                    Application.Exit();
                    return;
                }

                if (args.Cancelled)
                {
                    Application.Exit();
                    return;
                }

                var sectionResult = (SectionFeatureLoadResult)args.Result;

                this.sectionsService.AddSections(sectionResult.Sections);

                foreach (var f in sectionResult.Features)
                {
                    this.featureService.AddFeature(f);
                }

                this.featureService.NotifyChanges();

                if (sectionResult.Errors.Count > 0 || sectionResult.FileErrors.Count > 0)
                {
                    var hpisList = sectionResult.Errors.Select(x => x.HpiPath);
                    var filesList = sectionResult.FileErrors.Select(x => x.HpiPath + "\\" + x.FeaturePath);
                    this.dialogService.ShowError("Failed to load the following files:\n\n"
                        + string.Join("\n", hpisList) + "\n"
                        + string.Join("\n", filesList));
                }

                dlg.Close();
            };

            dlg.CancelPressed += (sender, args) => worker.CancelAsync();

            dlg.MessageText = "Loading sections and features ...";
            worker.RunWorkerAsync();

            dlg.Display();
        }

        public void HideGrid()
        {
            this.model.GridVisible = false;
        }

        public void EnableGridWithSize(Size s)
        {
            this.model.GridSize = s;
            this.model.GridVisible = true;
        }

        public void ChooseColor()
        {
            Color? c = this.dialogService.AskUserGridColor(this.model.GridColor);
            if (c.HasValue)
            {
                this.model.GridColor = c.Value;
            }
        }

        public void ShowAbout()
        {
            this.dialogService.ShowAbout();
        }

        public void Undo()
        {
            this.model.Map?.Undo();
        }

        public void Redo()
        {
            this.model.Map?.Redo();
        }

        public void New()
        {
            if (!this.CheckOkayDiscard())
            {
                return;
            }

            Size size = this.dialogService.AskUserNewMapSize();
            if (size.Width == 0 || size.Height == 0)
            {
                return;
            }

            this.New(size.Width, size.Height);
        }

        public void Open()
        {
            if (!this.CheckOkayDiscard())
            {
                return;
            }

            string filename = this.dialogService.AskUserToOpenFile();
            if (string.IsNullOrEmpty(filename))
            {
                return;
            }

            this.OpenMap(filename);
        }

        public void OpenFromDragDrop(string filename)
        {
            if (!this.CheckOkayDiscard())
            {
                return;
            }

            this.OpenMap(filename);
        }

        public void Save()
        {
            this.model.Map?.Save();
        }

        public void SaveAs()
        {
            this.model.Map?.SaveAs();
        }

        public void OpenPreferences()
        {
            this.dialogService.CapturePreferences();
        }

        public void Close()
        {
            if (this.CheckOkayDiscard())
            {
                Application.Exit();
            }
        }

        public void CloseMap()
        {
            if (this.CheckOkayDiscard())
            {
                this.model.Map = null;
            }
        }

        public void DragDropSection(int sectionId, int x, int y)
        {
            var section = this.sectionsService.Get(sectionId).GetTile();
            this.model.Map?.DragDropTile(section, x, y);
        }

        public void CopySelectionToClipboard()
        {
            this.model.Map?.CopySelectionToClipboard();
        }

        public void CutSelectionToClipboard()
        {
            this.model.Map?.CutSelectionToClipboard();
        }

        public void PasteFromClipboard()
        {
            if (this.model.Map == null)
            {
                return;
            }

            var loc = this.model.Map.ViewportLocation;
            loc.X += this.model.ViewportWidth / 2;
            loc.Y += this.model.ViewportHeight / 2;

            this.model.Map.PasteFromClipboard(loc.X, loc.Y);
        }

        public void RefreshMinimap()
        {
            this.model.Map?.RefreshMinimap();
        }

        public void RefreshMinimapHighQualityWithProgress()
        {
            this.model.Map?.RefreshMinimapHighQualityWithProgress();
        }

        public void ExportHeightmap()
        {
            this.model.Map?.ExportHeightmap();
        }

        public void ExportMinimap()
        {
            var loc = this.dialogService.AskUserToSaveMinimap();
            if (loc == null)
            {
                return;
            }

            try
            {
                using (var s = File.Create(loc))
                {
                    this.model.Map.Minimap.Save(s, ImageFormat.Png);
                }
            }
            catch (Exception)
            {
                this.dialogService.ShowError("There was a problem saving the minimap.");
            }
        }

        public void ExportMapImage()
        {
            this.model.Map?.ExportMapImage();
        }

        public void ImportCustomSection()
        {
            this.model.Map?.ImportCustomSection();
        }

        public void ImportHeightmap()
        {
            this.model.Map?.ImportHeightmap();
        }

        public void ImportMinimap()
        {
            this.model.Map?.ImportMinimap();
        }

        public void ToggleFeatures()
        {
            this.model.FeaturesVisible = !this.model.FeaturesVisible;
        }

        public void ToggleHeightmap()
        {
            this.model.HeightmapVisible = !this.model.HeightmapVisible;
        }

        public void ToggleMinimap()
        {
            this.model.MinimapVisible = !this.model.MinimapVisible;
        }

        public void OpenMapAttributes()
        {
            this.model.Map?.OpenMapAttributes();
        }

        public void SetSeaLevel(int value)
        {
            this.model.Map?.SetSeaLevel(value);
        }

        public void FlushSeaLevel()
        {
            this.model.Map?.FlushSeaLevel();
        }

        public void HideMinimap()
        {
            this.model.MinimapVisible = false;
        }

        public void SetViewportLocation(Point location)
        {
            this.model.SetViewportLocation(location);
        }

        public void SetViewportSize(Size size)
        {
            this.model.SetViewportSize(size);
        }

        public void SetStartPosition(int positionNumber, int x, int y)
        {
            this.model.Map?.DragDropStartPosition(positionNumber, x, y);
        }

        public void DragDropFeature(string featureName, int x, int y)
        {
            this.model.Map?.DragDropFeature(featureName, x, y);
        }

        public void DeleteSelection()
        {
            this.model.Map?.DeleteSelection();
        }

        public void ClearSelection()
        {
            this.model.Map?.ClearSelection();
        }

        public void DragDropStartPosition(int index, int x, int y)
        {
            this.model.Map?.DragDropStartPosition(index, x, y);
        }

        public void DragDropTile(IMapTile tile, int x, int y)
        {
            this.model.Map?.DragDropTile(tile, x, y);
        }

        public void StartBandbox(int x, int y)
        {
            this.model.Map?.StartBandbox(x, y);
        }

        public void GrowBandbox(int x, int y)
        {
            this.model.Map?.GrowBandbox(x, y);
        }

        public void CommitBandbox()
        {
            this.model.Map?.CommitBandbox();
        }

        public void TranslateSelection(int x, int y)
        {
            this.model.Map?.TranslateSelection(x, y);
        }

        public void FlushTranslation()
        {
            this.model.Map?.FlushTranslation();
        }

        public void SelectTile(int index)
        {
            this.model.Map?.SelectTile(index);
        }

        public void SelectFeature(Guid id)
        {
            this.model.Map?.SelectFeature(id);
        }

        public void SelectStartPosition(int index)
        {
            this.model.Map?.SelectStartPosition(index);
        }

        private static IEnumerable<string> GetMapNames(HpiReader hpi)
        {
            return hpi.GetFiles("maps")
                .Where(x => x.Name.EndsWith(".tnt", StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Name.Substring(0, x.Name.Length - 4));
        }

        private void OpenMap(string filename)
        {
            string ext = Path.GetExtension(filename) ?? string.Empty;
            ext = ext.ToUpperInvariant();

            try
            {
                switch (ext)
                {
                    case ".HPI":
                    case ".UFO":
                    case ".CCX":
                    case ".GPF":
                    case ".GP3":
                        this.OpenFromHapi(filename);
                        return;
                    case ".TNT":
                        this.OpenTnt(filename);
                        return;
                    case ".SCT":
                        this.OpenSct(filename);
                        return;
                    default:
                        this.dialogService.ShowError($"Mappy doesn't know how to open {ext} files");
                        return;
                }
            }
            catch (IOException e)
            {
                this.dialogService.ShowError("IO error opening map: " + e.Message);
            }
            catch (ParseException e)
            {
                this.dialogService.ShowError("Cannot open map: " + e.Message);
            }
        }

        private void OpenFromHapi(string filename)
        {
            List<string> maps;
            bool readOnly;

            using (HpiReader h = new HpiReader(filename))
            {
                maps = GetMapNames(h).ToList();
            }

            string mapName;
            switch (maps.Count)
            {
                case 0:
                    this.dialogService.ShowError("No maps found in " + filename);
                    return;
                case 1:
                    mapName = maps.First();
                    readOnly = false;
                    break;
                default:
                    maps.Sort();
                    mapName = this.dialogService.AskUserToChooseMap(maps);
                    readOnly = true;
                    break;
            }

            if (mapName == null)
            {
                return;
            }

            this.OpenHapi(filename, HpiPath.Combine("maps", mapName + ".tnt"), readOnly);
        }

        private void OpenHapi(string hpipath, string mappath, bool readOnly = false)
        {
            MapModel m;

            using (HpiReader hpi = new HpiReader(hpipath))
            {
                string otaPath = HpiPath.ChangeExtension(mappath, ".ota");

                TdfNode n;

                using (var ota = hpi.ReadTextFile(otaPath))
                {
                    n = TdfNode.LoadTdf(ota);
                }

                using (var s = new TntReader(hpi.ReadFile(mappath)))
                {
                    m = this.mapModelFactory.FromTntAndOta(s, n);
                }
            }

            this.model.Map = new UndoableMapModel(m, this.dialogService, hpipath, readOnly);
        }

        private void OpenTnt(string filename)
        {
            MapModel m;

            var otaFileName = filename.Substring(0, filename.Length - 4) + ".ota";
            if (File.Exists(otaFileName))
            {
                TdfNode attrs;
                using (var ota = File.OpenRead(otaFileName))
                {
                    attrs = TdfNode.LoadTdf(ota);
                }

                using (var s = new TntReader(filename))
                {
                    m = this.mapModelFactory.FromTntAndOta(s, attrs);
                }
            }
            else
            {
                using (var s = new TntReader(filename))
                {
                    m = this.mapModelFactory.FromTnt(s);
                }
            }

            this.model.Map = new UndoableMapModel(m, this.dialogService, filename, false);
        }

        private bool CheckOkayDiscard()
        {
            if (this.model.Map == null || this.model.Map.IsMarked)
            {
                return true;
            }

            DialogResult r = this.dialogService.AskUserToDiscardChanges();
            switch (r)
            {
                case DialogResult.Yes:
                    return this.model.Map.Save();
                case DialogResult.Cancel:
                    return false;
                case DialogResult.No:
                    return true;
                default:
                    throw new InvalidOperationException("unexpected dialog result: " + r);
            }
        }

        private void New(int width, int height)
        {
            var map = new MapModel(width, height);
            GridMethods.Fill(map.Tile.TileGrid, Globals.DefaultTile);
            var mapModel = new UndoableMapModel(map, this.dialogService, null, false);
            this.model.Map = mapModel;
        }

        private void OpenSct(string filename)
        {
            MapTile t;
            using (var s = new SctReader(filename))
            {
                t = this.sectionFactory.TileFromSct(s);
            }

            this.model.Map = new UndoableMapModel(new MapModel(t), this.dialogService, filename, true);
        }
    }
}
