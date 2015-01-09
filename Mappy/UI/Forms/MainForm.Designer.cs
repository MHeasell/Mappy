namespace Mappy.UI.Forms
{
    using Controls;

    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.closeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.importMinimapMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importHeightmapMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importCustomSectionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.exportMinimapMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportHeightmapMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportMapImageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.cutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.mapAttributesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.generateMinimapMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateMinimapHighQualityMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.preferencesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleHeightmapMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleMinimapMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gridMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gridOffMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.grid16MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grid32MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grid64MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grid128MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grid256MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grid512MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grid1024MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.gridColorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleFeaturesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.imageLayerView1 = new Mappy.UI.Controls.ImageLayerView();
            this.sectionView1 = new Mappy.UI.Controls.SectionView();
            this.featureview1 = new Mappy.UI.Controls.FeatureView();
            this.startPositionsView1 = new Mappy.UI.Controls.StartPositionsView();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem,
            this.editMenuItem,
            this.viewMenuItem,
            this.helpMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileMenuItem
            // 
            this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newMenuItem,
            this.openMenuItem,
            this.toolStripSeparator1,
            this.saveMenuItem,
            this.saveAsMenuItem,
            this.toolStripSeparator2,
            this.closeMenuItem,
            this.toolStripSeparator8,
            this.importMinimapMenuItem,
            this.importHeightmapMenuItem,
            this.importCustomSectionMenuItem,
            this.toolStripSeparator11,
            this.exportMinimapMenuItem,
            this.exportHeightmapMenuItem,
            this.exportMapImageMenuItem,
            this.toolStripSeparator10,
            this.exitMenuItem});
            this.fileMenuItem.Name = "fileMenuItem";
            this.fileMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileMenuItem.Text = "&File";
            // 
            // newMenuItem
            // 
            this.newMenuItem.Name = "newMenuItem";
            this.newMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newMenuItem.Size = new System.Drawing.Size(206, 22);
            this.newMenuItem.Text = "&New...";
            this.newMenuItem.Click += new System.EventHandler(this.NewMenuItemClick);
            // 
            // openMenuItem
            // 
            this.openMenuItem.Name = "openMenuItem";
            this.openMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openMenuItem.Size = new System.Drawing.Size(206, 22);
            this.openMenuItem.Text = "&Open...";
            this.openMenuItem.Click += new System.EventHandler(this.OpenMenuItemClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(203, 6);
            // 
            // saveMenuItem
            // 
            this.saveMenuItem.Enabled = false;
            this.saveMenuItem.Name = "saveMenuItem";
            this.saveMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveMenuItem.Size = new System.Drawing.Size(206, 22);
            this.saveMenuItem.Text = "&Save";
            this.saveMenuItem.Click += new System.EventHandler(this.SaveMenuItemClick);
            // 
            // saveAsMenuItem
            // 
            this.saveAsMenuItem.Enabled = false;
            this.saveAsMenuItem.Name = "saveAsMenuItem";
            this.saveAsMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.saveAsMenuItem.Size = new System.Drawing.Size(206, 22);
            this.saveAsMenuItem.Text = "Save &As...";
            this.saveAsMenuItem.Click += new System.EventHandler(this.SaveAsMenuItemClick);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(203, 6);
            // 
            // closeMenuItem
            // 
            this.closeMenuItem.Enabled = false;
            this.closeMenuItem.Name = "closeMenuItem";
            this.closeMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.closeMenuItem.Size = new System.Drawing.Size(206, 22);
            this.closeMenuItem.Text = "&Close";
            this.closeMenuItem.Click += new System.EventHandler(this.CloseMenuItemClick);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(203, 6);
            // 
            // importMinimapMenuItem
            // 
            this.importMinimapMenuItem.Enabled = false;
            this.importMinimapMenuItem.Name = "importMinimapMenuItem";
            this.importMinimapMenuItem.Size = new System.Drawing.Size(206, 22);
            this.importMinimapMenuItem.Text = "Import Minimap...";
            this.importMinimapMenuItem.Click += new System.EventHandler(this.ImportMinimapMenuItemClick);
            // 
            // importHeightmapMenuItem
            // 
            this.importHeightmapMenuItem.Enabled = false;
            this.importHeightmapMenuItem.Name = "importHeightmapMenuItem";
            this.importHeightmapMenuItem.Size = new System.Drawing.Size(206, 22);
            this.importHeightmapMenuItem.Text = "Import Heightmap...";
            this.importHeightmapMenuItem.Click += new System.EventHandler(this.ImportHeightmapMenuItemClick);
            // 
            // importCustomSectionMenuItem
            // 
            this.importCustomSectionMenuItem.Enabled = false;
            this.importCustomSectionMenuItem.Name = "importCustomSectionMenuItem";
            this.importCustomSectionMenuItem.Size = new System.Drawing.Size(206, 22);
            this.importCustomSectionMenuItem.Text = "Import Custom Section...";
            this.importCustomSectionMenuItem.Click += new System.EventHandler(this.ImportCustomSectionMenuItemClick);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(203, 6);
            // 
            // exportMinimapMenuItem
            // 
            this.exportMinimapMenuItem.Enabled = false;
            this.exportMinimapMenuItem.Name = "exportMinimapMenuItem";
            this.exportMinimapMenuItem.Size = new System.Drawing.Size(206, 22);
            this.exportMinimapMenuItem.Text = "Export Minimap...";
            this.exportMinimapMenuItem.Click += new System.EventHandler(this.ExportMinimapMenuItemClick);
            // 
            // exportHeightmapMenuItem
            // 
            this.exportHeightmapMenuItem.Enabled = false;
            this.exportHeightmapMenuItem.Name = "exportHeightmapMenuItem";
            this.exportHeightmapMenuItem.Size = new System.Drawing.Size(206, 22);
            this.exportHeightmapMenuItem.Text = "Export Heightmap...";
            this.exportHeightmapMenuItem.Click += new System.EventHandler(this.ExportHeightmapMenuItemClick);
            // 
            // exportMapImageMenuItem
            // 
            this.exportMapImageMenuItem.Enabled = false;
            this.exportMapImageMenuItem.Name = "exportMapImageMenuItem";
            this.exportMapImageMenuItem.Size = new System.Drawing.Size(206, 22);
            this.exportMapImageMenuItem.Text = "Export Map Image...";
            this.exportMapImageMenuItem.Click += new System.EventHandler(this.ExportMapImageMenuItemClick);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(203, 6);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(206, 22);
            this.exitMenuItem.Text = "E&xit";
            this.exitMenuItem.Click += new System.EventHandler(this.ExitMenuItemClick);
            // 
            // editMenuItem
            // 
            this.editMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoMenuItem,
            this.redoMenuItem,
            this.toolStripSeparator7,
            this.cutMenuItem,
            this.copyMenuItem,
            this.pasteMenuItem,
            this.toolStripSeparator9,
            this.mapAttributesMenuItem,
            this.toolStripSeparator3,
            this.generateMinimapMenuItem,
            this.generateMinimapHighQualityMenuItem,
            this.toolStripSeparator4,
            this.preferencesMenuItem});
            this.editMenuItem.Name = "editMenuItem";
            this.editMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editMenuItem.Text = "&Edit";
            // 
            // undoMenuItem
            // 
            this.undoMenuItem.Enabled = false;
            this.undoMenuItem.Name = "undoMenuItem";
            this.undoMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoMenuItem.Size = new System.Drawing.Size(250, 22);
            this.undoMenuItem.Text = "&Undo";
            this.undoMenuItem.Click += new System.EventHandler(this.UndoMenuItemClick);
            // 
            // redoMenuItem
            // 
            this.redoMenuItem.Enabled = false;
            this.redoMenuItem.Name = "redoMenuItem";
            this.redoMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoMenuItem.Size = new System.Drawing.Size(250, 22);
            this.redoMenuItem.Text = "&Redo";
            this.redoMenuItem.Click += new System.EventHandler(this.RedoMenuItemClick);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(247, 6);
            // 
            // cutMenuItem
            // 
            this.cutMenuItem.Enabled = false;
            this.cutMenuItem.Name = "cutMenuItem";
            this.cutMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutMenuItem.Size = new System.Drawing.Size(250, 22);
            this.cutMenuItem.Text = "Cut";
            this.cutMenuItem.Click += new System.EventHandler(this.CutMenuItemClick);
            // 
            // copyMenuItem
            // 
            this.copyMenuItem.Enabled = false;
            this.copyMenuItem.Name = "copyMenuItem";
            this.copyMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyMenuItem.Size = new System.Drawing.Size(250, 22);
            this.copyMenuItem.Text = "Copy";
            this.copyMenuItem.Click += new System.EventHandler(this.CopyMenuItemClick);
            // 
            // pasteMenuItem
            // 
            this.pasteMenuItem.Enabled = false;
            this.pasteMenuItem.Name = "pasteMenuItem";
            this.pasteMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteMenuItem.Size = new System.Drawing.Size(250, 22);
            this.pasteMenuItem.Text = "Paste";
            this.pasteMenuItem.Click += new System.EventHandler(this.PasteMenuItemClick);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(247, 6);
            // 
            // mapAttributesMenuItem
            // 
            this.mapAttributesMenuItem.Enabled = false;
            this.mapAttributesMenuItem.Name = "mapAttributesMenuItem";
            this.mapAttributesMenuItem.Size = new System.Drawing.Size(250, 22);
            this.mapAttributesMenuItem.Text = "Map Attributes...";
            this.mapAttributesMenuItem.Click += new System.EventHandler(this.MapAttributesMenuItemClick);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(247, 6);
            // 
            // generateMinimapMenuItem
            // 
            this.generateMinimapMenuItem.Enabled = false;
            this.generateMinimapMenuItem.Name = "generateMinimapMenuItem";
            this.generateMinimapMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.generateMinimapMenuItem.Size = new System.Drawing.Size(250, 22);
            this.generateMinimapMenuItem.Text = "&Generate Minimap";
            this.generateMinimapMenuItem.Click += new System.EventHandler(this.GenerateMinimapMenuItemClick);
            // 
            // generateMinimapHighQualityMenuItem
            // 
            this.generateMinimapHighQualityMenuItem.Enabled = false;
            this.generateMinimapHighQualityMenuItem.Name = "generateMinimapHighQualityMenuItem";
            this.generateMinimapHighQualityMenuItem.Size = new System.Drawing.Size(250, 22);
            this.generateMinimapHighQualityMenuItem.Text = "Generate Minimap (High Quality)";
            this.generateMinimapHighQualityMenuItem.Click += new System.EventHandler(this.GenerateMinimapHighQualityMenuItemClick);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(247, 6);
            // 
            // preferencesMenuItem
            // 
            this.preferencesMenuItem.Name = "preferencesMenuItem";
            this.preferencesMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.preferencesMenuItem.Size = new System.Drawing.Size(250, 22);
            this.preferencesMenuItem.Text = "&Preferences...";
            this.preferencesMenuItem.Click += new System.EventHandler(this.PreferencesMenuItemClick);
            // 
            // viewMenuItem
            // 
            this.viewMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toggleHeightmapMenuItem,
            this.toggleMinimapMenuItem,
            this.gridMenuItem,
            this.toggleFeaturesMenuItem});
            this.viewMenuItem.Name = "viewMenuItem";
            this.viewMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewMenuItem.Text = "&View";
            // 
            // toggleHeightmapMenuItem
            // 
            this.toggleHeightmapMenuItem.CheckOnClick = true;
            this.toggleHeightmapMenuItem.Name = "toggleHeightmapMenuItem";
            this.toggleHeightmapMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.toggleHeightmapMenuItem.Size = new System.Drawing.Size(177, 22);
            this.toggleHeightmapMenuItem.Text = "&Heightmap";
            this.toggleHeightmapMenuItem.CheckedChanged += new System.EventHandler(this.ToggleHeightmapMenuItemClick);
            // 
            // toggleMinimapMenuItem
            // 
            this.toggleMinimapMenuItem.Name = "toggleMinimapMenuItem";
            this.toggleMinimapMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.toggleMinimapMenuItem.Size = new System.Drawing.Size(177, 22);
            this.toggleMinimapMenuItem.Text = "&Minimap";
            this.toggleMinimapMenuItem.Click += new System.EventHandler(this.ToggleMinimapMenuItemClick);
            // 
            // gridMenuItem
            // 
            this.gridMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gridOffMenuItem,
            this.toolStripSeparator5,
            this.grid16MenuItem,
            this.grid32MenuItem,
            this.grid64MenuItem,
            this.grid128MenuItem,
            this.grid256MenuItem,
            this.grid512MenuItem,
            this.grid1024MenuItem,
            this.toolStripSeparator6,
            this.gridColorMenuItem});
            this.gridMenuItem.Name = "gridMenuItem";
            this.gridMenuItem.Size = new System.Drawing.Size(177, 22);
            this.gridMenuItem.Text = "Grid";
            // 
            // gridOffMenuItem
            // 
            this.gridOffMenuItem.Checked = true;
            this.gridOffMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.gridOffMenuItem.Name = "gridOffMenuItem";
            this.gridOffMenuItem.Size = new System.Drawing.Size(155, 22);
            this.gridOffMenuItem.Tag = "0";
            this.gridOffMenuItem.Text = "Off";
            this.gridOffMenuItem.Click += new System.EventHandler(this.GridMenuItemClick);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(152, 6);
            // 
            // grid16MenuItem
            // 
            this.grid16MenuItem.Name = "grid16MenuItem";
            this.grid16MenuItem.Size = new System.Drawing.Size(155, 22);
            this.grid16MenuItem.Tag = "16";
            this.grid16MenuItem.Text = "16x16";
            this.grid16MenuItem.Click += new System.EventHandler(this.GridMenuItemClick);
            // 
            // grid32MenuItem
            // 
            this.grid32MenuItem.Name = "grid32MenuItem";
            this.grid32MenuItem.Size = new System.Drawing.Size(155, 22);
            this.grid32MenuItem.Tag = "32";
            this.grid32MenuItem.Text = "32x32";
            this.grid32MenuItem.Click += new System.EventHandler(this.GridMenuItemClick);
            // 
            // grid64MenuItem
            // 
            this.grid64MenuItem.Name = "grid64MenuItem";
            this.grid64MenuItem.Size = new System.Drawing.Size(155, 22);
            this.grid64MenuItem.Tag = "64";
            this.grid64MenuItem.Text = "64x64";
            this.grid64MenuItem.Click += new System.EventHandler(this.GridMenuItemClick);
            // 
            // grid128MenuItem
            // 
            this.grid128MenuItem.Name = "grid128MenuItem";
            this.grid128MenuItem.Size = new System.Drawing.Size(155, 22);
            this.grid128MenuItem.Tag = "128";
            this.grid128MenuItem.Text = "128x128";
            this.grid128MenuItem.Click += new System.EventHandler(this.GridMenuItemClick);
            // 
            // grid256MenuItem
            // 
            this.grid256MenuItem.Name = "grid256MenuItem";
            this.grid256MenuItem.Size = new System.Drawing.Size(155, 22);
            this.grid256MenuItem.Tag = "256";
            this.grid256MenuItem.Text = "256x256";
            this.grid256MenuItem.Click += new System.EventHandler(this.GridMenuItemClick);
            // 
            // grid512MenuItem
            // 
            this.grid512MenuItem.Name = "grid512MenuItem";
            this.grid512MenuItem.Size = new System.Drawing.Size(155, 22);
            this.grid512MenuItem.Tag = "512";
            this.grid512MenuItem.Text = "512x512";
            this.grid512MenuItem.Click += new System.EventHandler(this.GridMenuItemClick);
            // 
            // grid1024MenuItem
            // 
            this.grid1024MenuItem.Name = "grid1024MenuItem";
            this.grid1024MenuItem.Size = new System.Drawing.Size(155, 22);
            this.grid1024MenuItem.Tag = "1024";
            this.grid1024MenuItem.Text = "1024x1024";
            this.grid1024MenuItem.Click += new System.EventHandler(this.GridMenuItemClick);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(152, 6);
            // 
            // gridColorMenuItem
            // 
            this.gridColorMenuItem.Name = "gridColorMenuItem";
            this.gridColorMenuItem.Size = new System.Drawing.Size(155, 22);
            this.gridColorMenuItem.Text = "Choose Color...";
            this.gridColorMenuItem.Click += new System.EventHandler(this.GridColorMenuItemClick);
            // 
            // toggleFeaturesMenuItem
            // 
            this.toggleFeaturesMenuItem.Checked = true;
            this.toggleFeaturesMenuItem.CheckOnClick = true;
            this.toggleFeaturesMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toggleFeaturesMenuItem.Name = "toggleFeaturesMenuItem";
            this.toggleFeaturesMenuItem.Size = new System.Drawing.Size(177, 22);
            this.toggleFeaturesMenuItem.Text = "Features";
            this.toggleFeaturesMenuItem.Click += new System.EventHandler(this.ToggleFeaturesMenuItemClick);
            // 
            // helpMenuItem
            // 
            this.helpMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutMenuItem});
            this.helpMenuItem.Name = "helpMenuItem";
            this.helpMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpMenuItem.Text = "&Help";
            // 
            // aboutMenuItem
            // 
            this.aboutMenuItem.Name = "aboutMenuItem";
            this.aboutMenuItem.Size = new System.Drawing.Size(152, 22);
            this.aboutMenuItem.Text = "&About...";
            this.aboutMenuItem.Click += new System.EventHandler(this.AboutMenuItemClick);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(215, 538);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.sectionView1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage1.Size = new System.Drawing.Size(207, 512);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Sections";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.featureview1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage2.Size = new System.Drawing.Size(207, 512);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Features";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.startPositionsView1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage3.Size = new System.Drawing.Size(207, 512);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Starts";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.label3);
            this.tabPage4.Controls.Add(this.label2);
            this.tabPage4.Controls.Add(this.trackBar1);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabPage4.Size = new System.Drawing.Size(207, 512);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Attributes";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Enabled = false;
            this.label3.Location = new System.Drawing.Point(173, 5);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 14);
            this.label3.TabIndex = 10;
            this.label3.Text = "0";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(4, 5);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Sea Level";
            // 
            // trackBar1
            // 
            this.trackBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar1.Enabled = false;
            this.trackBar1.Location = new System.Drawing.Point(62, 5);
            this.trackBar1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.trackBar1.Maximum = 255;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(106, 45);
            this.trackBar1.TabIndex = 8;
            this.trackBar1.TickFrequency = 16;
            this.trackBar1.ValueChanged += new System.EventHandler(this.TrackBar1ValueChanged);
            this.trackBar1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBar1_MouseUp);
            // 
            // imageLayerView1
            // 
            this.imageLayerView1.AllowDrop = true;
            this.imageLayerView1.CanvasColor = System.Drawing.Color.CornflowerBlue;
            this.imageLayerView1.CanvasSize = new System.Drawing.Size(0, 0);
            this.imageLayerView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageLayerView1.GridColor = System.Drawing.Color.Black;
            this.imageLayerView1.GridSize = new System.Drawing.Size(16, 16);
            this.imageLayerView1.GridVisible = false;
            this.imageLayerView1.Location = new System.Drawing.Point(215, 24);
            this.imageLayerView1.Name = "imageLayerView1";
            this.imageLayerView1.Size = new System.Drawing.Size(569, 538);
            this.imageLayerView1.TabIndex = 5;
            this.imageLayerView1.Text = "imageLayerView1";
            this.imageLayerView1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.imageLayerView1_Scroll);
            this.imageLayerView1.ClientSizeChanged += new System.EventHandler(this.MapPanel1SizeChanged);
            // 
            // sectionView1
            // 
            this.sectionView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sectionView1.Location = new System.Drawing.Point(3, 3);
            this.sectionView1.Margin = new System.Windows.Forms.Padding(4);
            this.sectionView1.Name = "sectionView1";
            this.sectionView1.Sections = null;
            this.sectionView1.Size = new System.Drawing.Size(201, 506);
            this.sectionView1.TabIndex = 3;
            // 
            // featureview1
            // 
            this.featureview1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.featureview1.Features = null;
            this.featureview1.Location = new System.Drawing.Point(3, 3);
            this.featureview1.Margin = new System.Windows.Forms.Padding(4);
            this.featureview1.Name = "featureview1";
            this.featureview1.Size = new System.Drawing.Size(201, 506);
            this.featureview1.TabIndex = 0;
            // 
            // startPositionsView1
            // 
            this.startPositionsView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.startPositionsView1.Location = new System.Drawing.Point(3, 3);
            this.startPositionsView1.Margin = new System.Windows.Forms.Padding(4);
            this.startPositionsView1.Name = "startPositionsView1";
            this.startPositionsView1.Size = new System.Drawing.Size(201, 506);
            this.startPositionsView1.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.imageLayerView1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Mappy";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1FormClosing);
            this.Load += new System.EventHandler(this.MainFormLoad);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toggleHeightmapMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editMenuItem;
        private System.Windows.Forms.ToolStripMenuItem preferencesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toggleMinimapMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoMenuItem;
        private SectionView sectionView1;
        private System.Windows.Forms.ToolStripMenuItem newMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem generateMinimapMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private FeatureView featureview1;
        private System.Windows.Forms.ToolStripMenuItem gridMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gridOffMenuItem;
        private System.Windows.Forms.ToolStripMenuItem grid16MenuItem;
        private System.Windows.Forms.ToolStripMenuItem grid32MenuItem;
        private System.Windows.Forms.ToolStripMenuItem grid64MenuItem;
        private System.Windows.Forms.ToolStripMenuItem grid128MenuItem;
        private System.Windows.Forms.ToolStripMenuItem grid256MenuItem;
        private System.Windows.Forms.ToolStripMenuItem grid512MenuItem;
        private System.Windows.Forms.ToolStripMenuItem grid1024MenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem gridColorMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toggleFeaturesMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem mapAttributesMenuItem;
        private System.Windows.Forms.TabPage tabPage3;
        private StartPositionsView startPositionsView1;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.ToolStripMenuItem closeMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem generateMinimapHighQualityMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem cutMenuItem;
        public ImageLayerView imageLayerView1;
        private System.Windows.Forms.ToolStripMenuItem exportMinimapMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripMenuItem exportHeightmapMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importMinimapMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importHeightmapMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripMenuItem exportMapImageMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importCustomSectionMenuItem;
    }
}

