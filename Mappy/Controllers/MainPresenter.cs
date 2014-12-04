namespace Mappy.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    using Mappy.Data;
    using Mappy.IO;
    using Mappy.Minimap;
    using Mappy.Palette;

    using Models;

    using TAUtil;
    using TAUtil.Hpi;

    using Views;

    /// <summary>
    /// Presenter for Mappy's main form.
    /// This is essentially the top-level presenter.
    /// </summary>
    public class MainPresenter
    {
        private const string ProgramName = "Mappy";

        private readonly IMainView view;

        private readonly IMainModel model;

        private readonly IMinimapModel minimapModel;

        public MainPresenter(IMainView view, CoreModel model)
        {
            this.view = view;
            this.model = model;
            this.minimapModel = model;

            this.view.UndoEnabled = this.model.CanUndo;
            this.view.RedoEnabled = this.model.CanRedo;

            this.view.CopyEnabled = this.model.CanCopy;
            this.view.CutEnabled = this.model.CanCut;
            this.view.PasteEnabled = this.model.CanPaste;

            this.model.PropertyChanged += this.CoreModelPropertyChanged;
            this.minimapModel.PropertyChanged += this.MinimapModelPropertyChanged;
        }

        public void Initialize()
        {
            var dlg = this.view.CreateProgressView();
            dlg.Title = "Loading Mappy";
            dlg.ShowProgress = true;
            dlg.CancelEnabled = true;

            var worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += delegate(object sender, DoWorkEventArgs args)
                {
                    var w = (BackgroundWorker)sender;
                    var p = (IPalette)args.Argument;
                    LoadResult<Section> result;
                    if (!SectionLoadingUtils.LoadSections(
                            p,
                            i => w.ReportProgress((50 * i) / 100),
                            () => w.CancellationPending,
                            out result))
                    {
                        args.Cancel = true;
                        return;
                    }

                    LoadResult<Feature> featureResult;
                    if (!FeatureLoadingUtils.LoadFeatures(
                        p,
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
                        this.model.Sections.Add(s);
                    }

                    this.view.Sections = sectionResult.Sections;

                    foreach (var f in sectionResult.Features)
                    {
                        this.model.FeatureRecords.AddFeature(f);
                    }

                    this.view.Features = this.model.FeatureRecords.EnumerateAll().ToList();

                    if (sectionResult.Errors.Count > 0 || sectionResult.FileErrors.Count > 0)
                    {
                        var hpisList = sectionResult.Errors.Select(x => x.HpiPath);
                        var filesList = sectionResult.FileErrors.Select(x => x.HpiPath + "\\" + x.FeaturePath);
                        this.view.ShowError("Failed to load the following files:\n\n"
                            + string.Join("\n", hpisList) + "\n"
                            + string.Join("\n", filesList));
                    }

                    dlg.Close();
                };

            dlg.CancelPressed += (sender, args) => worker.CancelAsync();

            dlg.MessageText = "Loading sections and features ...";
            worker.RunWorkerAsync(Globals.Palette);

            dlg.Display();
        }

        public bool Open()
        {
            if (!this.CheckOkayDiscard())
            {
                return false;
            }

            string filename = this.view.AskUserToOpenFile();
            if (string.IsNullOrEmpty(filename))
            {
                return false;
            }

            return this.OpenMap(filename);
        }

        public void OpenPressed(object sender, EventArgs e)
        {
            this.Open();
        }

        public bool Save()
        {
            if (this.model.FilePath == null || this.model.IsFileReadOnly)
            {
                return this.SaveAs();
            }

            return this.SaveHelper(this.model.FilePath);
        }

        public bool SaveAs()
        {
            string path = this.view.AskUserToSaveFile();

            if (path == null)
            {
                return false;
            }

            return this.SaveHelper(path);
        }

        public void SavePressed(object sender, EventArgs e)
        {
            this.Save();
        }

        public void SaveAsPressed(object sender, EventArgs e)
        {
            this.SaveAs();
        }

        public void UndoPressed(object sender, EventArgs e)
        {
            this.model.Undo();
        }

        public void RedoPressed(object sender, EventArgs e)
        {
            this.model.Redo();
        }

        public void PreferencesPressed(object sender, EventArgs e)
        {
            this.view.CapturePreferences();
        }

        public void ClosePressed(object sender, EventArgs e)
        {
            if (this.CheckOkayDiscard())
            {
                this.view.Close();
            }
        }

        public bool CheckOkayDiscard()
        {
            if (!this.model.IsDirty)
            {
                return true;
            }

            DialogResult r = this.view.AskUserToDiscardChanges();
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

        public void ToggleFeatures()
        {
            this.model.FeaturesVisible = !this.model.FeaturesVisible;
        }

        public void ToggleHeightmap()
        {
            this.model.HeightmapVisible = !this.model.HeightmapVisible;
        }

        public bool New()
        {
            if (!this.CheckOkayDiscard())
            {
                return false;
            }

            Size size = this.view.AskUserNewMapSize();
            if (size.Width == 0 || size.Height == 0)
            {
                return false;
            }

            this.model.New(size.Width, size.Height);
            return true;
        }

        public void GenerateMinimapPressed(object sender, EventArgs e)
        {
            this.model.RefreshMinimap();
        }

        public void GenerateMinimapHiqhQualityPressed(object sender, EventArgs e)
        {
            if (this.model.Map == null)
            {
                return;
            }

            var worker = Mappy.Util.Util.RenderMinimapWorker();

            var dlg = this.view.CreateProgressView();
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
                        this.model.SetMinimap(img);
                    }

                    dlg.Close();
                };

            worker.RunWorkerAsync(this.model.Map);
            dlg.Display();
        }

        public void SetGridSize(int size)
        {
            if (size == 0)
            {
                this.model.GridVisible = false;
            }
            else
            {
                this.model.GridVisible = true;
                this.model.GridSize = new Size(size, size);
            }
        }

        public void ChooseColor()
        {
            Color? c = this.view.AskUserGridColor(this.model.GridColor);
            if (c.HasValue)
            {
                this.model.GridColor = c.Value;
            }
        }

        public void ExportMinimap()
        {
            var loc = this.view.AskUserToSaveMinimap();
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
                this.view.ShowError("There was a problem saving the minimap.");
            }
        }

        public void OpenMapAttributes()
        {
            MapAttributesResult r = this.view.AskUserForMapAttributes(this.model.GetAttributes());

            if (r != null)
            {
                this.model.UpdateAttributes(r);
            }
        }

        public void ToggleMinimap()
        {
            this.minimapModel.MinimapVisible = !this.minimapModel.MinimapVisible;
        }

        public void UpdateMinimapViewport()
        {
            this.minimapModel.ViewportRectangle = this.ConvertToNormalizedViewport(this.view.ViewportRect);
        }

        public void SetSeaLevel(int value)
        {
            if (this.model.MapOpen)
            {
                this.model.SetSeaLevel(value);
            }
        }

        public void FlushSeaLevel()
        {
            this.model.FlushSeaLevel();
        }

        public void CloseMap()
        {
            if (this.CheckOkayDiscard())
            {
                this.model.CloseMap();
            }
        }

        public void CutToClipboard()
        {
            this.model.CutSelectionToClipboard();
        }

        public void CopyToClipboard()
        {
            this.model.CopySelectionToClipboard();
        }

        public void PasteFromClipboard()
        {
            this.model.PasteFromClipboard();
        }

        private RectangleF ConvertToNormalizedViewport(Rectangle rect)
        {
            if (!this.model.MapOpen)
            {
                return RectangleF.Empty;
            }

            var widthScale = (float)((this.model.MapWidth * 32) - 32);
            var heightScale = (float)((this.model.MapHeight * 32) - 128);

            var x = rect.X / widthScale;
            var y = rect.Y / heightScale;
            var w = rect.Width / widthScale;
            var h = rect.Height / heightScale;

            return new RectangleF(x, y, w, h);
        }

        private bool SaveHelper(string filename)
        {
            if (filename == null)
            {
                throw new ArgumentNullException("filename");
            }

            string extension = Path.GetExtension(filename).ToLowerInvariant();

            try
            {
                switch (extension)
                {
                    case ".tnt":
                        this.model.Save(filename);
                        return true;
                    case ".hpi":
                    case ".ufo":
                    case ".ccx":
                    case ".gpf":
                    case ".gp3":
                        this.model.SaveHpi(filename);
                        return true;
                    default:
                        this.view.ShowError("Unrecognized file extension: " + extension);
                        return false;
                }
            }
            catch (IOException e)
            {
                this.view.ShowError("Error saving map: " + e.Message);
                return false;
            }
        }

        private bool OpenMap(string filename)
        {
            string ext = Path.GetExtension(filename) ?? string.Empty;
            ext = ext.ToLowerInvariant();

            try
            {
                switch (ext)
                {
                    case ".hpi":
                    case ".ufo":
                    case ".ccx":
                    case ".gpf":
                    case ".gp3":
                        return this.OpenFromHapi(filename);
                    case ".tnt":
                        this.model.OpenTnt(filename);
                        return true;
                    case ".sct":
                        this.model.OpenSct(filename);
                        return true;
                    default:
                        this.view.ShowError(string.Format("Mappy doesn't know how to open {0} files", ext));
                        return false;
                }
            }
            catch (IOException e)
            {
                this.view.ShowError("IO error opening map: " + e.Message);
                return false;
            }
            catch (ParseException e)
            {
                this.view.ShowError("Cannot open map: " + e.Message);
                return false;
            }
        }

        private IEnumerable<string> GetMapNames(HpiReader hpi)
        {
            return hpi.GetFiles("maps")
                .Where(x => x.Name.EndsWith(".tnt", StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Name.Substring(0, x.Name.Length - 4));
        }

        private bool OpenFromHapi(string filename)
        {
            List<string> maps;
            bool readOnly;

            using (HpiReader h = new HpiReader(filename))
            {
                maps = this.GetMapNames(h).ToList();
            }

            string mapName;
            switch (maps.Count)
            {
                case 0:
                    this.view.ShowError("No maps found in " + filename);
                    return false;
                case 1:
                    mapName = maps.First();
                    readOnly = false;
                    break;
                default:
                    maps.Sort();
                    mapName = this.view.AskUserToChooseMap(maps);
                    readOnly = true;
                    break;
            }

            if (mapName == null)
            {
                return false;
            }

            this.model.OpenHapi(filename, HpiPath.Combine("maps", mapName + ".tnt"), readOnly);
            return true;
        }

        private void CoreModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CanUndo":
                    this.view.UndoEnabled = this.model.CanUndo;
                    break;
                case "CanRedo":
                    this.view.RedoEnabled = this.model.CanRedo;
                    break;
                case "CanCopy":
                    this.view.CopyEnabled = this.model.CanCopy;
                    break;
                case "CanCut":
                    this.view.CutEnabled = this.model.CanCut;
                    break;
                case "CanPaste":
                    this.view.PasteEnabled = this.model.CanPaste;
                    break;
                case "MapOpen":
                    this.UpdateSave();
                    this.view.OpenAttributesEnabled = this.model.MapOpen;
                    this.UpdateMinimapViewport();
                    this.view.CloseEnabled = this.model.MapOpen;
                    this.view.SeaLevelEditEnabled = this.model.MapOpen;
                    this.view.RefreshMinimapEnabled = this.model.MapOpen;
                    this.view.RefreshMinimapHighQualityEnabled = this.model.MapOpen;
                    this.view.ExportMinimapEnabled = this.model.MapOpen;
                    break;
                case "IsFileOpen":
                    this.view.SaveAsEnabled = this.model.IsFileOpen;
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
                    this.view.SeaLevel = this.model.SeaLevel;
                    break;
            }
        }

        private void MinimapModelPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            switch (propertyChangedEventArgs.PropertyName)
            {
                case "MinimapVisible":
                    this.view.MinimapVisibleChecked = this.minimapModel.MinimapVisible;
                    break;
            }
        }

        private void UpdateSave()
        {
            this.view.SaveEnabled = this.model.MapOpen && this.model.FilePath != null && !this.model.IsFileReadOnly;
        }

        private void UpdateTitleText()
        {
            this.view.TitleText = this.GenerateTitleText();
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

        public class SectionFeatureLoadResult
        {
            public IList<Section> Sections { get; set; }

            public IList<Feature> Features { get; set; }

            public List<HpiErrorInfo> Errors { get; set; }

            public List<HpiInnerFileErrorInfo> FileErrors { get; set; }
        }
    }
}
