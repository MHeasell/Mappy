namespace Mappy.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

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

        private readonly CoreModel model;

        public MainPresenter(IMainView view, CoreModel model)
        {
            this.view = view;
            this.model = model;

            this.view.Features = this.model.FeatureRecords.EnumerateAll().ToList();

            this.view.Sections = this.model.Sections;

            this.view.UndoEnabled = this.model.CanUndo;
            this.view.RedoEnabled = this.model.CanRedo;

            this.model.PropertyChanged += this.CoreModelPropertyChanged;
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
            if (this.model.FilePath == null)
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

        public void OpenMapAttributes()
        {
            MapAttributesResult r = this.view.AskUserForMapAttributes(MapAttributesResult.FromModel(this.model.Map));

            if (r != null)
            {
                this.model.UpdateAttributes(r);
            }
        }

        public void SetSelectionMode(BandboxMode mode)
        {
            this.model.SelectionMode = mode;
        }

        public void ToggleMinimap()
        {
            this.model.MinimapVisible = !this.model.MinimapVisible;
        }

        public void UpdateMinimapViewport()
        {
            this.model.ViewportRectangle = this.ConvertToNormalizedViewport(this.view.ViewportRect);
        }

        private RectangleF ConvertToNormalizedViewport(Rectangle rect)
        {
            if (this.model.Map == null)
            {
                return RectangleF.Empty;
            }

            var widthScale = (float)(this.model.Map.Tile.TileGrid.Width * 32);
            var heightScale = (float)(this.model.Map.Tile.TileGrid.Height * 32);

            var x = rect.X / widthScale;
            var y = rect.Y / heightScale;
            var w = rect.Width / widthScale;
            var h = rect.Height / heightScale;

            return new RectangleF(x, y, w, h);
        }

        private bool SaveHelper(string filename)
        {
            string extension = Path.GetExtension(filename);

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
            ext = ext.ToLower();

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
                this.view.ShowError(string.Format("IO error opening map: " + e.Message));
                return false;
            }
            catch (ParseException e)
            {
                this.view.ShowError(string.Format("Cannot open map: " + e.Message));
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

            this.model.OpenHapi(filename, Path.Combine("maps", mapName + ".tnt"), readOnly);
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
                case "Map":
                    this.view.OpenAttributesEnabled = this.model.Map != null;
                    this.UpdateMinimapViewport();
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
                case "MinimapVisible":
                    this.view.MinimapVisibleChecked = this.model.MinimapVisible;
                    break;
            }
        }

        private void UpdateSave()
        {
            this.view.SaveEnabled = this.model.FilePath != null && !this.model.IsFileReadOnly;
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
    }
}
