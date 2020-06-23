namespace Mappy.Services
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    using Mappy.Models;
    using Mappy.UI.Forms;
    using Mappy.Views;

    public class DialogService : IDialogService
    {
        private readonly Form owner;

        public DialogService(Form owner)
        {
            this.owner = owner;
        }

        public SectionImportPaths AskUserToChooseSectionImportPaths()
        {
            var dlg = new ImportCustomSectionForm();
            var result = dlg.ShowDialog(this.owner);
            if (result != DialogResult.OK)
            {
                return null;
            }

            return new SectionImportPaths
            {
                GraphicPath = dlg.GraphicPath,
                HeightmapPath = dlg.HeightmapPath
            };
        }

        public string AskUserToChooseMap(IList<string> maps)
        {
            var f = new MapSelectionForm();
            foreach (var n in maps)
            {
                f.Items.Add(n);
            }

            var r = f.ShowDialog(this.owner);
            if (r == DialogResult.OK)
            {
                return (string)f.SelectedItem;
            }

            return null;
        }

        public string AskUserToOpenFile()
        {
            var d = new OpenFileDialog();
            d.Filter = "TA Map Files|*.hpi;*.ufo;*.ccx;*.gpf;*.gp3;*.tnt|All files|*.*";
            if (d.ShowDialog(this.owner) == DialogResult.OK)
            {
                return d.FileName;
            }

            return null;
        }

        public string AskUserToChooseMinimap()
        {
            var d = new OpenFileDialog();
            d.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All files|*.*";
            if (d.ShowDialog(this.owner) == DialogResult.OK)
            {
                return d.FileName;
            }

            return null;
        }

        public string AskUserToSaveFile()
        {
            var d = new SaveFileDialog();
            d.Filter = "HPI files|*.hpi;*.ufo;*.ccx;*.gpf;*.gp3|TNT files|*.tnt|All files|*.*";
            d.AddExtension = true;
            var result = d.ShowDialog(this.owner);
            if (result == DialogResult.OK)
            {
                return d.FileName;
            }

            return null;
        }

        public string AskUserToSaveMinimap()
        {
            var d = new SaveFileDialog();
            d.Title = "Export Minimap";
            d.Filter = "PNG files|*.png|All files|*.*";
            d.AddExtension = true;
            var result = d.ShowDialog(this.owner);
            if (result == DialogResult.OK)
            {
                return d.FileName;
            }

            return null;
        }

        public string AskUserToSaveHeightmap()
        {
            var d = new SaveFileDialog();
            d.Title = "Export Heightmap";
            d.Filter = "PNG files|*.png|All files|*.*";
            d.AddExtension = true;
            var result = d.ShowDialog(this.owner);
            if (result == DialogResult.OK)
            {
                return d.FileName;
            }

            return null;
        }

        public string AskUserToSaveMapImage()
        {
            var d = new SaveFileDialog();
            d.Title = "Export Map Image";
            d.Filter = "PNG files|*.png|All files|*.*";
            d.AddExtension = true;
            var result = d.ShowDialog(this.owner);
            if (result == DialogResult.OK)
            {
                return d.FileName;
            }

            return null;
        }

        public string AskUserToChooseHeightmap(int width, int height)
        {
            var d = new OpenFileDialog();
            d.Title = $"Import Heightmap ({width}x{height} image)";
            d.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All files|*.*";
            if (d.ShowDialog(this.owner) == DialogResult.OK)
            {
                return d.FileName;
            }

            return null;
        }

        public DialogResult AskUserToDiscardChanges()
        {
            return MessageBox.Show("There are unsaved changes. Save before closing?", "Save", MessageBoxButtons.YesNoCancel);
        }

        public Size AskUserNewMapSize()
        {
            var dialog = new NewMapForm();
            var result = dialog.ShowDialog(this.owner);

            switch (result)
            {
                case DialogResult.OK:
                    return new Size(dialog.MapWidth, dialog.MapHeight);
                case DialogResult.Cancel:
                    return Size.Empty;
                default:
                    throw new ArgumentException("bad dialogresult");
            }
        }

        public Color? AskUserGridColor(Color previousColor)
        {
            var colorDialog = new ColorDialog();
            colorDialog.Color = previousColor;
            var result = colorDialog.ShowDialog(this.owner);

            if (result == DialogResult.OK)
            {
                return colorDialog.Color;
            }

            return null;
        }

        public MapAttributesResult AskUserForMapAttributes(MapAttributesResult r)
        {
            var f = new MapAttributesForm();

            f.mapAttributesResultBindingSource.Add(r);

            var result = f.ShowDialog(this.owner);
            if (result == DialogResult.OK)
            {
                return r;
            }

            return null;
        }

        public void ShowError(string message)
        {
            MessageBox.Show(this.owner, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public IProgressView CreateProgressView()
        {
            var dlg = new ProgressForm();
            dlg.Owner = this.owner;
            return dlg;
        }

        public void CapturePreferences()
        {
            var f = new PreferencesForm();
            f.ShowDialog();
        }

        public void ShowAbout()
        {
            var f = new AboutForm();
            f.ShowDialog(this.owner);
        }
    }
}
