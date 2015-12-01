namespace Mappy.Models
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
    using Mappy.Database;
    using Mappy.IO;
    using Mappy.Util;

    using TAUtil;
    using TAUtil.Hpi;
    using TAUtil.Sct;
    using TAUtil.Tdf;
    using TAUtil.Tnt;

    public class CoreModel : Notifier, IMapViewSettingsModel, IUserEventDispatcher
    {
        private readonly IFeatureDatabase featureRecords;
        private readonly IList<Section> sections;

        private readonly SectionFactory sectionFactory;

        private readonly MapModelFactory mapModelFactory;

        private readonly IDialogService dialogService;

        private UndoableMapModel map;

        private bool heightmapVisible;
        private bool featuresVisible = true;

        private bool minimapVisible;

        private bool gridVisible;
        private Size gridSize = new Size(16, 16);
        private Color gridColor = MappySettings.Settings.GridColor;

        private int viewportWidth;
        private int viewportHeight;

        public CoreModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;

            this.featureRecords = new FeatureDictionary();
            this.sections = new List<Section>();

            this.sectionFactory = new SectionFactory();
            this.mapModelFactory = new MapModelFactory();
        }

        IMainModel IMapViewSettingsModel.Map
        {
            get
            {
                return this.Map;
            }
        }

        public bool MapOpen
        {
            get
            {
                return this.Map != null;
            }
        }

        public UndoableMapModel Map
        {
            get
            {
                return this.map;
            }

            private set
            {
                if (this.SetField(ref this.map, value, "Map"))
                {
                    if (this.Map != null)
                    {
                        this.Map.PropertyChanged += this.MapOnPropertyChanged;
                    }

                    this.FireChange("MapOpen");
                    this.FireChange("MapWidth");
                    this.FireChange("MapHeight");
                    this.FireChange("SeaLevel");

                    this.FireChange("SelectedTile");
                    this.FireChange("SelectedStartPosition");
                    this.FireChange("SelectedFeatures");

                    this.FireChange("CanUndo");
                    this.FireChange("CanRedo");

                    this.FireChange("IsDirty");
                    this.FireChange("FilePath");
                    this.FireChange("IsFileReadOnly");

                    this.FireChange("CanCut");
                    this.FireChange("CanCopy");
                    this.FireChange("CanPaste");

                    this.FireChange("MinimapImage");
                }
            }
        }

        public int SeaLevel
        {
            get
            {
                return this.Map == null ? 0 : this.Map.SeaLevel;
            }
        }

        public bool CanUndo
        {
            get { return this.Map != null && this.Map.CanUndo; }
        }

        public bool CanRedo
        {
            get { return this.Map != null && this.Map.CanRedo; }
        }

        public bool CanCopy
        {
            get
            {
                return this.Map != null && this.Map.CanCopy;
            }
        }

        public bool CanPaste
        {
            get
            {
                return this.Map != null;
            }
        }

        public bool CanCut
        {
            get
            {
                return this.Map != null && this.Map.CanCut;
            }
        }

        public IFeatureDatabase FeatureRecords
        {
            get { return this.featureRecords; }
        }

        public IList<Section> Sections
        {
            get { return this.sections; }
        } 

        public bool IsDirty
        {
            get { return this.Map != null && !this.Map.IsMarked; }
        }

        public string FilePath
        {
            get { return this.Map == null ? null : this.Map.FilePath; }
        }

        public bool IsFileReadOnly
        {
            get { return this.Map != null && this.Map.IsFileReadOnly; }
        }

        public bool HeightmapVisible
        {
            get { return this.heightmapVisible; }
            set { this.SetField(ref this.heightmapVisible, value, "HeightmapVisible"); }
        }

        public bool FeaturesVisible
        {
            get { return this.featuresVisible; }
            set { this.SetField(ref this.featuresVisible, value, "FeaturesVisible"); }
        }

        public int ViewportWidth
        {
            get
            {
                return this.viewportWidth;
            }

            set
            {
                this.SetField(ref this.viewportWidth, value, "ViewportWidth");
            }
        }

        public int ViewportHeight
        {
            get
            {
                return this.viewportHeight;
            }

            set
            {
                this.SetField(ref this.viewportHeight, value, "ViewportHeight");
            }
        }

        public Point ViewportLocation
        {
            get
            {
                return this.Map == null ? Point.Empty : this.Map.ViewportLocation;
            }
        }

        public int MapWidth
        {
            get
            {
                return this.Map == null ? 0 : this.Map.BaseTile.TileGrid.Width;
            }
        }

        public int MapHeight
        {
            get
            {
                return this.Map == null ? 0 : this.Map.BaseTile.TileGrid.Height;
            }
        }

        public bool MinimapVisible
        {
            get
            {
                return this.minimapVisible;
            }

            set
            {
                this.SetField(ref this.minimapVisible, value, "MinimapVisible");
            }
        }

        public bool GridVisible
        {
            get { return this.gridVisible; }
            set { this.SetField(ref this.gridVisible, value, "GridVisible"); }
        }

        public Size GridSize
        {
            get { return this.gridSize; }
            set { this.SetField(ref this.gridSize, value, "GridSize"); }
        }

        public Color GridColor
        {
            get
            {
                return this.gridColor;
            }

            set
            {
                MappySettings.Settings.GridColor = value;
                MappySettings.SaveSettings();
                this.SetField(ref this.gridColor, value, "GridColor");
            }
        }

        public Bitmap MinimapImage
        {
            get
            {
                return this.Map == null ? null : this.Map.Minimap;
            }
        }

        public void HideGrid()
        {
            this.GridVisible = false;
        }

        public void EnableGridWithSize(Size s)
        {
            this.GridSize = s;
            this.GridVisible = true;
        }

        public void ShowAbout()
        {
            this.dialogService.ShowAbout();
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

                int nextId = 0;
                foreach (var s in sectionResult.Sections)
                {
                    s.Id = nextId++;
                    this.Sections.Add(s);
                }

                this.FireChange("Sections");

                foreach (var f in sectionResult.Features)
                {
                    this.FeatureRecords.AddFeature(f);
                }

                this.FireChange("FeatureRecords");

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

        public void Undo()
        {
            if (this.Map == null)
            {
                return;
            }

            this.Map.Undo();
        }

        public void Redo()
        {
            if (this.Map == null)
            {
                return;
            }
            
            this.Map.Redo();
        }

        public bool New()
        {
            if (!this.CheckOkayDiscard())
            {
                return false;
            }

            Size size = this.dialogService.AskUserNewMapSize();
            if (size.Width == 0 || size.Height == 0)
            {
                return false;
            }

            this.New(size.Width, size.Height);
            return true;
        }

        public bool Open()
        {
            if (!this.CheckOkayDiscard())
            {
                return false;
            }

            string filename = this.dialogService.AskUserToOpenFile();
            if (string.IsNullOrEmpty(filename))
            {
                return false;
            }

            return this.OpenMap(filename);
        }

        public bool OpenFromDragDrop(string filename)
        {
            if (!this.CheckOkayDiscard())
            {
                return false;
            }

            return this.OpenMap(filename);
        }

        public bool Save()
        {
            if (this.Map == null)
            {
                return false;
            }

            return this.Map.Save();
        }

        public bool SaveAs()
        {
            if (this.Map == null)
            {
                return false;
            }

            return this.Map.SaveAs();
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

        public void CopySelectionToClipboard()
        {
            if (this.Map == null)
            {
                return;
            }

            this.Map.CopySelectionToClipboard();
        }

        public void CutSelectionToClipboard()
        {
            if (this.Map == null)
            {
                return;
            }

            this.Map.CutSelectionToClipboard();
        }

        public void PasteFromClipboard()
        {
            if (this.Map == null)
            {
                return;
            }

            var loc = this.ViewportLocation;
            loc.X += this.ViewportWidth / 2;
            loc.Y += this.ViewportHeight / 2;

            this.Map.PasteFromClipboard(loc.X, loc.Y);
        }

        public void RefreshMinimap()
        {
            if (this.Map == null)
            {
                return;
            }

            this.Map.RefreshMinimap();
        }

        public void RefreshMinimapHighQualityWithProgress()
        {
            if (this.Map == null)
            {
                return;
            }

            this.Map.RefreshMinimapHighQualityWithProgress();
        }

        public void ChooseColor()
        {
            Color? c = this.dialogService.AskUserGridColor(this.GridColor);
            if (c.HasValue)
            {
                this.GridColor = c.Value;
            }
        }

        public void ExportHeightmap()
        {
            if (this.Map == null)
            {
                return;
            }

            this.Map.ExportHeightmap();
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
                    this.Map.Minimap.Save(s, ImageFormat.Png);
                }
            }
            catch (Exception)
            {
                this.dialogService.ShowError("There was a problem saving the minimap.");
            }
        }

        public void ExportMapImage()
        {
            if (this.Map == null)
            {
                return;
            }

            this.Map.ExportMapImage();
        }

        public void ImportCustomSection()
        {
            if (this.Map == null)
            {
                return;
            }

            this.Map.ImportCustomSection();
        }

        public void ImportHeightmap()
        {
            if (this.Map == null)
            {
                return;
            }

            this.Map.ImportHeightmap();
        }

        public void ImportMinimap()
        {
            if (this.Map == null)
            {
                return;
            }

            this.Map.ImportMinimap();
        }

        public void ToggleFeatures()
        {
            this.FeaturesVisible = !this.FeaturesVisible;
        }

        public void ToggleHeightmap()
        {
            this.HeightmapVisible = !this.HeightmapVisible;
        }

        public void ToggleMinimap()
        {
            this.MinimapVisible = !this.MinimapVisible;
        }

        public void OpenMapAttributes()
        {
            if (this.Map == null)
            {
                return;
            }

            this.Map.OpenMapAttributes();
        }

        public void CloseMap()
        {
            if (this.CheckOkayDiscard())
            {
                this.Map = null;
            }
        }

        public void SetSeaLevel(int value)
        {
            if (this.Map == null)
            {
                return;
            }

            this.Map.SetSeaLevel(value);
        }

        public void FlushSeaLevel()
        {
            if (this.Map == null)
            {
                return;
            }

            this.Map.FlushSeaLevel();
        }

        public void HideMinimap()
        {
            this.MinimapVisible = false;
        }

        public void SetViewportLocation(Point location)
        {
            if (this.Map == null)
            {
                return;
            }

            location.X = Util.Clamp(location.X, 0, (this.MapWidth * 32) - this.ViewportWidth);
            location.Y = Util.Clamp(location.Y, 0, (this.MapHeight * 32) - this.ViewportHeight);

            this.Map.ViewportLocation = location;
        }

        private static IEnumerable<string> GetMapNames(HpiReader hpi)
        {
            return hpi.GetFiles("maps")
                .Where(x => x.Name.EndsWith(".tnt", StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Name.Substring(0, x.Name.Length - 4));
        }

        private bool CheckOkayDiscard()
        {
            if (!this.IsDirty)
            {
                return true;
            }

            DialogResult r = this.dialogService.AskUserToDiscardChanges();
            switch (r)
            {
                case DialogResult.Yes:
                    return this.Save();
                case DialogResult.Cancel:
                    return false;
                case DialogResult.No:
                    return true;
                default:
                    throw new InvalidOperationException("unexpected dialog result: " + r);
            }
        }

        private void OpenSct(string filename)
        {
            MapTile t;
            using (var s = new SctReader(filename))
            {
                t = this.sectionFactory.TileFromSct(s);
            }

            this.SetMapModel(new MapModel(t), filename, true);
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

            this.SetMapModel(m, filename, false);
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

            this.SetMapModel(m, hpipath, readOnly);
        }

        private void SetMapModel(MapModel model, string path, bool readOnly)
        {
            this.Map = new UndoableMapModel(model, this.dialogService, path, readOnly);
        }

        private void New(int width, int height)
        {
            var map = new MapModel(width, height);
            GridMethods.Fill(map.Tile.TileGrid, Globals.DefaultTile);
            this.SetMapModel(map, null, false);
        }

        private bool OpenMap(string filename)
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
                        return this.OpenFromHapi(filename);
                    case ".TNT":
                        this.OpenTnt(filename);
                        return true;
                    case ".SCT":
                        this.OpenSct(filename);
                        return true;
                    default:
                        this.dialogService.ShowError(string.Format("Mappy doesn't know how to open {0} files", ext));
                        return false;
                }
            }
            catch (IOException e)
            {
                this.dialogService.ShowError("IO error opening map: " + e.Message);
                return false;
            }
            catch (ParseException e)
            {
                this.dialogService.ShowError("Cannot open map: " + e.Message);
                return false;
            }
        }

        private bool OpenFromHapi(string filename)
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
                    return false;
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
                return false;
            }

            this.OpenHapi(filename, HpiPath.Combine("maps", mapName + ".tnt"), readOnly);
            return true;
        }

        private void MapOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            switch (propertyChangedEventArgs.PropertyName)
            {
                case "Minimap":
                    this.FireChange("MinimapImage");
                    break;
                case "IsMarked":
                    this.FireChange("IsDirty");
                    break;
                case "SeaLevel":
                case "CanUndo":
                case "CanRedo":
                case "FilePath":
                case "IsFileReadOnly":
                case "CanCut":
                case "CanCopy":
                case "CanPaste":
                case "ViewportLocation":
                    this.FireChange(propertyChangedEventArgs.PropertyName);
                    break;
            }
        }
    }
}
