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

    using TAUtil.Hpi;

    using Views;

    public class MainPresenter
    {
        private const string ProgramName = "Mappy";

        private readonly IMainView view;

        private readonly CoreModel model;

        public MainPresenter(IMainView view, CoreModel model)
        {
            this.view = view;
            this.model = model;

            this.view.Presenter = this;

            this.view.Features = this.model.FeatureRecords.Values.ToList();

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

            this.SaveHelper(this.model.FilePath);
            return true;
        }

        public bool SaveAs()
        {
            string path = this.view.AskUserToSaveFile();

            if (path == null)
            {
                return false;
            }

            this.SaveHelper(path);
            return true;
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

        private void SaveHelper(string filename)
        {
            string extension = Path.GetExtension(filename);
            switch (extension)
            {
                case ".tnt":
                    this.model.Save(filename);
                    break;
                case ".hpi":
                    this.model.SaveHpi(filename);
                    break;
                default:
                    throw new ArgumentException("unknown extension: " + extension);
            }
        }

        private bool OpenMap(string filename)
        {
            string ext = Path.GetExtension(filename);
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
                    throw new ArgumentException("Unrecognised file extension: " + ext);
            }
        }

        private IEnumerable<string> GetMapNames(HpiReader hpi)
        {
            foreach (string mapFile in hpi.GetFiles("maps"))
            {
                if (mapFile.EndsWith(".tnt", StringComparison.OrdinalIgnoreCase))
                {
                    yield return mapFile.Substring(0, mapFile.Length - 4);
                }
            }
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
                    throw new ArgumentException("No maps found in " + filename);
                case 1:
                    mapName = maps.First();
                    readOnly = false;
                    break;
                default:
                    mapName = this.view.AskUserToChooseMap(maps);
                    readOnly = true;
                    break;
            }

            if (string.IsNullOrEmpty(mapName))
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
                    this.view.Map = this.model.Map;
                    this.view.OpenAttributesEnabled = this.model.Map != null;
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
