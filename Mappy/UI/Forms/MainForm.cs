namespace Mappy.UI.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;
    using Data;
    using Mappy.Controllers;
    using Mappy.Models;
    using Mappy.Views;

    public partial class MainForm : Form, IMainView
    {
        private MinimapForm minimapForm;

        private Point oldAutoScrollPos;

        private IList<Section> sections;
        private IList<Feature> features;

        private IBindingMapModel map;

        public MainForm()
        {
            this.InitializeComponent();

            // paint events don't seem to fire from mapPanel1 when it is scrolled,
            // so we also listen for paint events from the child mapcontrol
            // to tell us when it was scrolled
            this.imageLayerView1.Paint += this.MapPanel1Paint;

            CoreModel model = new CoreModel();

            new MapPresenter(this.imageLayerView1, model);
            new MainPresenter(this, model);
        }

        public event EventHandler ViewportLocationChanged;

        public MainPresenter Presenter { get; set; }

        public string TitleText
        {
            get { return this.Text; }
            set { this.Text = value; }
        }

        public bool MinimapVisible
        {
            get { return this.minimapForm.Visible; }
            set { this.minimapForm.Visible = value; }
        }

        public bool UndoEnabled
        {
            get { return this.undoToolStripMenuItem.Enabled; }
            set { this.undoToolStripMenuItem.Enabled = value; }
        }

        public bool RedoEnabled
        {
            get { return this.redoToolStripMenuItem.Enabled; }
            set { this.redoToolStripMenuItem.Enabled = value; }
        }

        public bool SaveEnabled
        {
            get
            {
                return this.toolStripMenuItem5.Enabled;
            }

            set
            {
                this.toolStripMenuItem5.Enabled = value;
            }
        }

        public bool SaveAsEnabled
        {
            get
            {
                return this.toolStripMenuItem4.Enabled;
            }

            set
            {
                this.toolStripMenuItem4.Enabled = value;
            }
        }

        public Rectangle ViewportRect
        {
            get
            {
                Point loc = this.imageLayerView1.AutoScrollPosition;
                loc.X *= -1;
                loc.Y *= -1;
                return new Rectangle(loc, this.imageLayerView1.ClientSize);
            }
        }

        public IBindingMapModel Map
        {
            get
            {
                return this.map;
            }

            set
            {
                this.map = value;
                this.minimapForm.Map = value;
            }
        }

        public IList<Section> Sections
        {
            get
            {
                return this.sections;
            }

            set
            {
                this.sections = value;
                this.sectionView1.Sections = value;
            }
        }

        public IList<Feature> Features
        {
            get
            {
                return this.features;
            }

            set
            {
                this.features = value;
                this.featureview1.Features = value;
            }
        }

        public bool OpenAttributesEnabled
        {
            get { return this.toolStripMenuItem11.Enabled; }
            set { this.toolStripMenuItem11.Enabled = value; }
        }

        public string AskUserToChooseMap(IList<string> maps)
        {
            MapSelectionForm f = new MapSelectionForm();
            foreach (string n in maps)
            {
                f.Items.Add(n);
            }

            DialogResult r = f.ShowDialog(this);
            if (r == DialogResult.OK)
            {
                return (string)f.SelectedItem;
            }

            return null;
        }

        public string AskUserToOpenFile()
        {
            OpenFileDialog d = new OpenFileDialog();
            d.Filter = "HAPI archives|*.hpi;*.ufo;*.ccx;*.gpf;*.gp3|TNT files|*.tnt|All files|*.*";
            if (d.ShowDialog(this) == DialogResult.OK)
            {
                return d.FileName;
            }

            return null;
        }

        public void SetViewportCenter(Point p)
        {
            p.X -= this.imageLayerView1.ClientSize.Width / 2;
            p.Y -= this.imageLayerView1.ClientSize.Height / 2;
            this.imageLayerView1.AutoScrollPosition = p;
        }

        public void CapturePreferences()
        {
            PreferencesForm f = new PreferencesForm();
            f.ShowDialog();
        }

        public string AskUserToSaveFile()
        {
            SaveFileDialog d = new SaveFileDialog();
            d.Filter = "HPI files|*.hpi;*.ufo;*.ccx;*.gpf;*.gp3|TNT files|*.tnt|All files|*.*";
            d.AddExtension = true;
            DialogResult result = d.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                return d.FileName;
            }

            return null;
        }

        void IMainView.Close()
        {
            Application.Exit();
        }

        public DialogResult AskUserToDiscardChanges()
        {
            return MessageBox.Show("There are unsaved changes. Save before closing?", "Save", MessageBoxButtons.YesNoCancel);
        }

        public Size AskUserNewMapSize()
        {
            NewMapForm dialog = new NewMapForm();
            DialogResult result = dialog.ShowDialog(this);

            switch (result)
            {
                case DialogResult.OK:
                    return new Size(dialog.MapWidth, dialog.MapHeight);
                case DialogResult.Cancel:
                    return new Size();
                default:
                    throw new ArgumentException("bad dialogresult");
            }
        }

        public Color? AskUserGridColor(Color previousColor)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = previousColor;
            DialogResult result = colorDialog.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                return colorDialog.Color;
            }

            return null;
        }

        public MapAttributesResult AskUserForMapAttributes(MapAttributesResult r)
        {
            MapAttributesForm f = new MapAttributesForm();

            f.mapAttributesResultBindingSource.Add(r);

            DialogResult result = f.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                return r;
            }

            return null;
        }

        public void ShowError(string message)
        {
            MessageBox.Show(this, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Form1Load(object sender, EventArgs e)
        {
            this.minimapForm = new MinimapForm();
            this.minimapForm.Owner = this;
            this.minimapForm.MainView = this;
            new MinimapController(this.minimapForm, this);
        }

        private void OpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Presenter.Open();
        }

        private void HeightmapToolStripMenuItemCheckedChanged(object sender, EventArgs e)
        {
            this.Presenter.ToggleHeightmap();
        }

        private void OnViewportChanged()
        {
            EventHandler h = this.ViewportLocationChanged;
            if (h != null)
            {
                h(this, EventArgs.Empty);
            }
        }

        private void MapPanel1Paint(object sender, PaintEventArgs e)
        {
            Point p = this.imageLayerView1.AutoScrollPosition;
            if (p != this.oldAutoScrollPos)
            {
                this.oldAutoScrollPos = p;
                this.OnViewportChanged();
            }
        }

        private void MapPanel1SizeChanged(object sender, EventArgs e)
        {
            this.OnViewportChanged();
        }

        private void PreferencesToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Presenter.PreferencesPressed(this, e);
        }

        private void ToolStripMenuItem4Click(object sender, EventArgs e)
        {
            this.Presenter.SaveAs();
        }

        private void ToolStripMenuItem5Click(object sender, EventArgs e)
        {
            this.Presenter.Save();
        }

        private void MinimapToolStripMenuItem1Click(object sender, EventArgs e)
        {
            this.MinimapVisible = this.minimapToolStripMenuItem1.Checked;
        }

        private void UndoToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Presenter.UndoPressed(this, e);
        }

        private void RedoToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Presenter.RedoPressed(this, e);
        }

        private void Form1FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Presenter.ClosePressed(this, EventArgs.Empty); 
                e.Cancel = true;
            }
        }

        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Presenter.ClosePressed(this, e);
        }

        private void ToolStripMenuItem2Click(object sender, EventArgs e)
        {
            this.Presenter.New();
        }

        private void AboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            AboutForm f = new AboutForm();
            f.ShowDialog(this);
        }

        private void ToolStripMenuItem6Click(object sender, EventArgs e)
        {
            this.Presenter.GenerateMinimapPressed(this, e);
        }

        private void ClearGridCheckboxes()
        {
            offToolStripMenuItem.Checked = false;
            toolStripMenuItem10.Checked = false;
            toolStripMenuItem9.Checked = false;
            toolStripMenuItem8.Checked = false;
            toolStripMenuItem7.Checked = false;
            x256ToolStripMenuItem.Checked = false;
            x512ToolStripMenuItem.Checked = false;
            x1024ToolStripMenuItem.Checked = false;
        }

        private void OffToolStripMenuItemClick(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            this.ClearGridCheckboxes();
            item.Checked = true;
            int size = Convert.ToInt32(item.Tag);

            this.Presenter.SetGridSize(size);
        }

        private void ChooseColorToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Presenter.ChooseColor();
        }

        private void FeaturesToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Presenter.ToggleFeatures();
        }

        private void ToolStripMenuItem11Click(object sender, EventArgs e)
        {
            this.Presenter.OpenMapAttributes();
        }
    }
}
