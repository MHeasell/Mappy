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
    using Mappy.Maybe;
    using Mappy.Models;
    using Mappy.Util;
    using Mappy.Util.ImageSampling;

    using TAUtil;
    using TAUtil.Gdi.Palette;
    using TAUtil.Hpi;
    using TAUtil.Tnt;

    public class Dispatcher
    {
        private readonly CoreModel model;

        private readonly IDialogService dialogService;

        private readonly SectionsService sectionsService;

        private readonly FeatureService featureService;

        private readonly MapLoadingService mapLoadingService;

        public Dispatcher(
            CoreModel model,
            IDialogService dialogService,
            SectionsService sectionsService,
            FeatureService featureService,
            MapLoadingService mapLoadingService)
        {
            this.model = model;
            this.dialogService = dialogService;
            this.sectionsService = sectionsService;
            this.featureService = featureService;
            this.mapLoadingService = mapLoadingService;
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
            var map = this.model.Map;
            if (map == null)
            {
                return;
            }

            Bitmap minimap;
            using (var adapter = new MapPixelImageAdapter(map.BaseTile.TileGrid))
            {
                minimap = Util.GenerateMinimap(adapter);
            }

            map.SetMinimap(minimap);
        }

        public void RefreshMinimapHighQualityWithProgress()
        {
            var map = this.model.Map;
            if (map == null)
            {
                return;
            }

            var worker = Mappy.Util.Util.RenderMinimapWorker();

            var dlg = this.dialogService.CreateProgressView();
            dlg.Title = "Generating Minimap";
            dlg.MessageText = "Generating high quality minimap...";

            dlg.CancelPressed += (o, args) => worker.CancelAsync();
            worker.ProgressChanged += (o, args) => dlg.Progress = args.ProgressPercentage;
            worker.RunWorkerCompleted += delegate(object o, RunWorkerCompletedEventArgs args)
            {
                if (args.Error != null)
                {
                    Program.HandleUnexpectedException(args.Error);
                    Application.Exit();
                    return;
                }

                if (!args.Cancelled)
                {
                    var img = (Bitmap)args.Result;
                    map.SetMinimap(img);
                }

                dlg.Close();
            };

            worker.RunWorkerAsync(this.model);
            dlg.Display();
        }

        public void ExportHeightmap()
        {
            if (this.model.Map == null)
            {
                return;
            }

            var loc = this.dialogService.AskUserToSaveHeightmap();
            if (loc == null)
            {
                return;
            }

            try
            {
                var b = Mappy.Util.Util.ExportHeightmap(this.model.Map.BaseTile.HeightGrid);
                using (var s = File.Create(loc))
                {
                    b.Save(s, ImageFormat.Png);
                }
            }
            catch (Exception)
            {
                this.dialogService.ShowError("There was a problem saving the heightmap.");
            }
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
            var map = this.model.Map;
            if (map == null)
            {
                return;
            }

            var loc = this.dialogService.AskUserToSaveMapImage();
            if (loc == null)
            {
                return;
            }

            var pv = this.dialogService.CreateProgressView();

            var tempLoc = loc + ".mappy-partial";

            var bg = new BackgroundWorker();
            bg.WorkerReportsProgress = true;
            bg.WorkerSupportsCancellation = true;
            bg.DoWork += delegate(object sender, DoWorkEventArgs args)
            {
                var worker = (BackgroundWorker)sender;
                using (var s = File.Create(tempLoc))
                {
                    var success = Mappy.Util.Util.WriteMapImage(s, map.BaseTile.TileGrid, worker.ReportProgress, () => worker.CancellationPending);
                    args.Cancel = !success;
                }
            };

            bg.ProgressChanged += (sender, args) => pv.Progress = args.ProgressPercentage;
            pv.CancelPressed += (sender, args) => bg.CancelAsync();

            bg.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs args)
            {
                try
                {
                    pv.Close();

                    if (args.Cancelled)
                    {
                        return;
                    }

                    if (args.Error != null)
                    {
                        this.dialogService.ShowError("There was a problem saving the map image.");
                        return;
                    }

                    if (File.Exists(loc))
                    {
                        File.Replace(tempLoc, loc, null);
                    }
                    else
                    {
                        File.Move(tempLoc, loc);
                    }
                }
                finally
                {
                    if (File.Exists(tempLoc))
                    {
                        File.Delete(tempLoc);
                    }
                }
            };

            bg.RunWorkerAsync();
            pv.Display();
        }

        public void ImportCustomSection()
        {
            var map = this.model.Map;
            if (map == null)
            {
                return;
            }

            var paths = this.dialogService.AskUserToChooseSectionImportPaths();
            if (paths == null)
            {
                return;
            }

            var dlg = this.dialogService.CreateProgressView();

            var bg = new BackgroundWorker();
            bg.WorkerSupportsCancellation = true;
            bg.WorkerReportsProgress = true;
            bg.DoWork += delegate(object sender, DoWorkEventArgs args)
            {
                var w = (BackgroundWorker)sender;
                var sect = ImageImport.ImportSection(
                    paths.GraphicPath,
                    paths.HeightmapPath,
                    w.ReportProgress,
                    () => w.CancellationPending);
                if (sect == null)
                {
                    args.Cancel = true;
                    return;
                }

                args.Result = sect;
            };

            bg.ProgressChanged += (sender, args) => dlg.Progress = args.ProgressPercentage;
            dlg.CancelPressed += (sender, args) => bg.CancelAsync();

            bg.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs args)
            {
                dlg.Close();

                if (args.Error != null)
                {
                    this.dialogService.ShowError(
                        "There was a problem importing the section: " + args.Error.Message);
                    return;
                }

                if (args.Cancelled)
                {
                    return;
                }

                map.PasteMapTileNoDeduplicateTopLeft((IMapTile)args.Result);
            };

            bg.RunWorkerAsync();

            dlg.Display();
        }

        public void ImportHeightmap()
        {
            var map = this.model.Map;
            if (map == null)
            {
                return;
            }

            var w = map.BaseTile.HeightGrid.Width;
            var h = map.BaseTile.HeightGrid.Height;

            var newHeightmap = this.LoadHeightmapFromUser(w, h);
            newHeightmap.IfSome(x => map.ReplaceHeightmap(x));
        }

        public void ImportMinimap()
        {
            if (this.model.Map == null)
            {
                return;
            }

            var minimap = this.LoadMinimapFromUser();
            minimap.IfSome(x => this.model.Map.SetMinimap(x));
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
            var map = this.model.Map;
            if (map == null)
            {
                return;
            }

            MapAttributesResult r = this.dialogService.AskUserForMapAttributes(map.GetAttributes());
            if (r != null)
            {
                map.UpdateAttributes(r);
            }
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

            var tntPath = HpiPath.Combine("maps", mapName + ".tnt");
            this.model.Map = this.mapLoadingService.CreateFromHpi(filename, tntPath, readOnly);
        }

        private void OpenTnt(string filename)
        {
            this.model.Map = this.mapLoadingService.CreateFromTnt(filename);
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
            this.model.Map = this.mapLoadingService.CreateMap(width, height);
        }

        private void OpenSct(string filename)
        {
            this.model.Map = this.mapLoadingService.CreateFromSct(filename);
        }

        private Maybe<Grid<int>> LoadHeightmapFromUser(int width, int height)
        {
            var loc = this.dialogService.AskUserToChooseHeightmap(width, height);
            if (loc == null)
            {
                return Maybe.None<Grid<int>>();
            }

            try
            {
                Bitmap bmp;
                using (var s = File.OpenRead(loc))
                {
                    bmp = (Bitmap)Image.FromStream(s);
                }

                if (bmp.Width != width || bmp.Height != height)
                {
                    var msg = string.Format(
                        "Heightmap has incorrect dimensions. The required dimensions are {0}x{1}.",
                        width,
                        height);
                    this.dialogService.ShowError(msg);
                    return Maybe.None<Grid<int>>();
                }

                return Maybe.Some(Mappy.Util.Util.ReadHeightmap(bmp));
            }
            catch (Exception)
            {
                this.dialogService.ShowError("There was a problem importing the selected heightmap");
                return Maybe.None<Grid<int>>();
            }
        }

        private Maybe<Bitmap> LoadMinimapFromUser()
        {
            var loc = this.dialogService.AskUserToChooseMinimap();
            if (loc == null)
            {
                return Maybe.None<Bitmap>();
            }

            try
            {
                Bitmap bmp;
                using (var s = File.OpenRead(loc))
                {
                    bmp = (Bitmap)Image.FromStream(s);
                }

                if (bmp.Width > TntConstants.MaxMinimapWidth
                    || bmp.Height > TntConstants.MaxMinimapHeight)
                {
                    var msg = string.Format(
                        "Minimap dimensions too large. The maximum size is {0}x{1}.",
                        TntConstants.MaxMinimapWidth,
                        TntConstants.MaxMinimapHeight);

                    this.dialogService.ShowError(msg);
                    return Maybe.None<Bitmap>();
                }

                Quantization.ToTAPalette(bmp);
                return Maybe.Some(bmp);
            }
            catch (Exception)
            {
                this.dialogService.ShowError("There was a problem importing the selected minimap.");
                return Maybe.None<Bitmap>();
            }
        }
    }
}
