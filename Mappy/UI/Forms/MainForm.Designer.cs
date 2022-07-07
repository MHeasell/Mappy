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
            System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.topMenu = new System.Windows.Forms.MenuStrip();
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importMinimapMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importHeightmapMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importCustomSectionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportMinimapMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportHeightmapMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportMapImageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fillMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapAttributesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateMinimapMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateMinimapHighQualityMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preferencesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleHeightmapMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleHeightGridMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleMinimapMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleVoidsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gridMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gridOffMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grid16MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grid32MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grid64MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grid128MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grid256MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grid512MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grid1024MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gridColorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleFeaturesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sidebarTabs = new System.Windows.Forms.TabControl();
            this.sectionsTab = new System.Windows.Forms.TabPage();
            this.sectionsView = new Mappy.UI.Controls.SectionView();
            this.featuresTab = new System.Windows.Forms.TabPage();
            this.featureView = new Mappy.UI.Controls.SectionView();
            this.startPositionsTab = new System.Windows.Forms.TabPage();
            this.startPositionsView1 = new Mappy.UI.Controls.StartPositionsView();
            this.attributesTab = new System.Windows.Forms.TabPage();
            this.seaLevelValueLabel = new System.Windows.Forms.Label();
            this.seaLevelLabel = new System.Windows.Forms.Label();
            this.seaLevelTrackbar = new System.Windows.Forms.TrackBar();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.mousePositionLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.hoveredFeatureLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.mapViewPanel = new Mappy.UI.Controls.MapViewPanel();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.topMenu.SuspendLayout();
            this.sidebarTabs.SuspendLayout();
            this.sectionsTab.SuspendLayout();
            this.featuresTab.SuspendLayout();
            this.startPositionsTab.SuspendLayout();
            this.attributesTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seaLevelTrackbar)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(308, 6);
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(308, 6);
            // 
            // toolStripSeparator8
            // 
            toolStripSeparator8.Name = "toolStripSeparator8";
            toolStripSeparator8.Size = new System.Drawing.Size(308, 6);
            // 
            // toolStripSeparator11
            // 
            toolStripSeparator11.Name = "toolStripSeparator11";
            toolStripSeparator11.Size = new System.Drawing.Size(308, 6);
            // 
            // toolStripSeparator10
            // 
            toolStripSeparator10.Name = "toolStripSeparator10";
            toolStripSeparator10.Size = new System.Drawing.Size(308, 6);
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Name = "toolStripSeparator7";
            toolStripSeparator7.Size = new System.Drawing.Size(370, 6);
            // 
            // toolStripSeparator9
            // 
            toolStripSeparator9.Name = "toolStripSeparator9";
            toolStripSeparator9.Size = new System.Drawing.Size(370, 6);
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(370, 6);
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new System.Drawing.Size(370, 6);
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new System.Drawing.Size(231, 6);
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new System.Drawing.Size(231, 6);
            // 
            // topMenu
            // 
            this.topMenu.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.topMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.topMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem,
            this.editMenuItem,
            this.viewMenuItem,
            this.helpMenuItem});
            this.topMenu.Location = new System.Drawing.Point(0, 0);
            this.topMenu.Name = "topMenu";
            this.topMenu.Size = new System.Drawing.Size(1176, 35);
            this.topMenu.TabIndex = 1;
            this.topMenu.Text = "menuStrip1";
            // 
            // fileMenuItem
            // 
            this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newMenuItem,
            this.openMenuItem,
            toolStripSeparator1,
            this.saveMenuItem,
            this.saveAsMenuItem,
            toolStripSeparator2,
            this.closeMenuItem,
            toolStripSeparator8,
            this.importMinimapMenuItem,
            this.importHeightmapMenuItem,
            this.importCustomSectionMenuItem,
            toolStripSeparator11,
            this.exportMinimapMenuItem,
            this.exportHeightmapMenuItem,
            this.exportMapImageMenuItem,
            toolStripSeparator10,
            this.exitMenuItem});
            this.fileMenuItem.Name = "fileMenuItem";
            this.fileMenuItem.Size = new System.Drawing.Size(54, 29);
            this.fileMenuItem.Text = "&File";
            // 
            // newMenuItem
            // 
            this.newMenuItem.Name = "newMenuItem";
            this.newMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newMenuItem.Size = new System.Drawing.Size(311, 34);
            this.newMenuItem.Text = "&New...";
            this.newMenuItem.Click += new System.EventHandler(this.NewMenuItemClick);
            // 
            // openMenuItem
            // 
            this.openMenuItem.Name = "openMenuItem";
            this.openMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openMenuItem.Size = new System.Drawing.Size(311, 34);
            this.openMenuItem.Text = "&Open...";
            this.openMenuItem.Click += new System.EventHandler(this.OpenMenuItemClick);
            // 
            // saveMenuItem
            // 
            this.saveMenuItem.Enabled = false;
            this.saveMenuItem.Name = "saveMenuItem";
            this.saveMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveMenuItem.Size = new System.Drawing.Size(311, 34);
            this.saveMenuItem.Text = "&Save";
            this.saveMenuItem.Click += new System.EventHandler(this.SaveMenuItemClick);
            // 
            // saveAsMenuItem
            // 
            this.saveAsMenuItem.Enabled = false;
            this.saveAsMenuItem.Name = "saveAsMenuItem";
            this.saveAsMenuItem.Size = new System.Drawing.Size(311, 34);
            this.saveAsMenuItem.Text = "Save &As...";
            this.saveAsMenuItem.Click += new System.EventHandler(this.SaveAsMenuItemClick);
            // 
            // closeMenuItem
            // 
            this.closeMenuItem.Enabled = false;
            this.closeMenuItem.Name = "closeMenuItem";
            this.closeMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.closeMenuItem.Size = new System.Drawing.Size(311, 34);
            this.closeMenuItem.Text = "&Close";
            this.closeMenuItem.Click += new System.EventHandler(this.CloseMenuItemClick);
            // 
            // importMinimapMenuItem
            // 
            this.importMinimapMenuItem.Enabled = false;
            this.importMinimapMenuItem.Name = "importMinimapMenuItem";
            this.importMinimapMenuItem.Size = new System.Drawing.Size(311, 34);
            this.importMinimapMenuItem.Text = "Import Minimap...";
            this.importMinimapMenuItem.Click += new System.EventHandler(this.ImportMinimapMenuItemClick);
            // 
            // importHeightmapMenuItem
            // 
            this.importHeightmapMenuItem.Enabled = false;
            this.importHeightmapMenuItem.Name = "importHeightmapMenuItem";
            this.importHeightmapMenuItem.Size = new System.Drawing.Size(311, 34);
            this.importHeightmapMenuItem.Text = "Import Heightmap...";
            this.importHeightmapMenuItem.Click += new System.EventHandler(this.ImportHeightmapMenuItemClick);
            // 
            // importCustomSectionMenuItem
            // 
            this.importCustomSectionMenuItem.Enabled = false;
            this.importCustomSectionMenuItem.Name = "importCustomSectionMenuItem";
            this.importCustomSectionMenuItem.Size = new System.Drawing.Size(311, 34);
            this.importCustomSectionMenuItem.Text = "Import Custom Section...";
            this.importCustomSectionMenuItem.Click += new System.EventHandler(this.ImportCustomSectionMenuItemClick);
            // 
            // exportMinimapMenuItem
            // 
            this.exportMinimapMenuItem.Enabled = false;
            this.exportMinimapMenuItem.Name = "exportMinimapMenuItem";
            this.exportMinimapMenuItem.Size = new System.Drawing.Size(311, 34);
            this.exportMinimapMenuItem.Text = "Export Minimap...";
            this.exportMinimapMenuItem.Click += new System.EventHandler(this.ExportMinimapMenuItemClick);
            // 
            // exportHeightmapMenuItem
            // 
            this.exportHeightmapMenuItem.Enabled = false;
            this.exportHeightmapMenuItem.Name = "exportHeightmapMenuItem";
            this.exportHeightmapMenuItem.Size = new System.Drawing.Size(311, 34);
            this.exportHeightmapMenuItem.Text = "Export Heightmap...";
            this.exportHeightmapMenuItem.Click += new System.EventHandler(this.ExportHeightmapMenuItemClick);
            // 
            // exportMapImageMenuItem
            // 
            this.exportMapImageMenuItem.Enabled = false;
            this.exportMapImageMenuItem.Name = "exportMapImageMenuItem";
            this.exportMapImageMenuItem.Size = new System.Drawing.Size(311, 34);
            this.exportMapImageMenuItem.Text = "Export Map Image...";
            this.exportMapImageMenuItem.Click += new System.EventHandler(this.ExportMapImageMenuItemClick);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(311, 34);
            this.exitMenuItem.Text = "E&xit";
            this.exitMenuItem.Click += new System.EventHandler(this.ExitMenuItemClick);
            // 
            // editMenuItem
            // 
            this.editMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoMenuItem,
            this.redoMenuItem,
            toolStripSeparator7,
            this.cutMenuItem,
            this.copyMenuItem,
            this.pasteMenuItem,
            this.fillMenuItem,
            toolStripSeparator9,
            this.mapAttributesMenuItem,
            toolStripSeparator3,
            this.generateMinimapMenuItem,
            this.generateMinimapHighQualityMenuItem,
            toolStripSeparator4,
            this.preferencesMenuItem});
            this.editMenuItem.Name = "editMenuItem";
            this.editMenuItem.Size = new System.Drawing.Size(58, 29);
            this.editMenuItem.Text = "&Edit";
            // 
            // undoMenuItem
            // 
            this.undoMenuItem.Enabled = false;
            this.undoMenuItem.Name = "undoMenuItem";
            this.undoMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoMenuItem.Size = new System.Drawing.Size(373, 34);
            this.undoMenuItem.Text = "&Undo";
            this.undoMenuItem.Click += new System.EventHandler(this.UndoMenuItemClick);
            // 
            // redoMenuItem
            // 
            this.redoMenuItem.Enabled = false;
            this.redoMenuItem.Name = "redoMenuItem";
            this.redoMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoMenuItem.Size = new System.Drawing.Size(373, 34);
            this.redoMenuItem.Text = "&Redo";
            this.redoMenuItem.Click += new System.EventHandler(this.RedoMenuItemClick);
            // 
            // cutMenuItem
            // 
            this.cutMenuItem.Enabled = false;
            this.cutMenuItem.Name = "cutMenuItem";
            this.cutMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutMenuItem.Size = new System.Drawing.Size(373, 34);
            this.cutMenuItem.Text = "Cut";
            this.cutMenuItem.Click += new System.EventHandler(this.CutMenuItemClick);
            // 
            // copyMenuItem
            // 
            this.copyMenuItem.Enabled = false;
            this.copyMenuItem.Name = "copyMenuItem";
            this.copyMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyMenuItem.Size = new System.Drawing.Size(373, 34);
            this.copyMenuItem.Text = "Copy";
            this.copyMenuItem.Click += new System.EventHandler(this.CopyMenuItemClick);
            // 
            // pasteMenuItem
            // 
            this.pasteMenuItem.Enabled = false;
            this.pasteMenuItem.Name = "pasteMenuItem";
            this.pasteMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteMenuItem.Size = new System.Drawing.Size(373, 34);
            this.pasteMenuItem.Text = "Paste";
            this.pasteMenuItem.Click += new System.EventHandler(this.PasteMenuItemClick);
            // 
            // fillMenuItem
            // 
            this.fillMenuItem.Enabled = false;
            this.fillMenuItem.Name = "fillMenuItem";
            this.fillMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.fillMenuItem.Size = new System.Drawing.Size(250, 22);
            this.fillMenuItem.Text = "Fill Map";
            this.fillMenuItem.Click += new System.EventHandler(this.FillMenuItemClick);
            // 
            // mapAttributesMenuItem
            // 
            this.mapAttributesMenuItem.Enabled = false;
            this.mapAttributesMenuItem.Name = "mapAttributesMenuItem";
            this.mapAttributesMenuItem.Size = new System.Drawing.Size(373, 34);
            this.mapAttributesMenuItem.Text = "Map Attributes...";
            this.mapAttributesMenuItem.Click += new System.EventHandler(this.MapAttributesMenuItemClick);
            // 
            // generateMinimapMenuItem
            // 
            this.generateMinimapMenuItem.Enabled = false;
            this.generateMinimapMenuItem.Name = "generateMinimapMenuItem";
            this.generateMinimapMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.generateMinimapMenuItem.Size = new System.Drawing.Size(373, 34);
            this.generateMinimapMenuItem.Text = "&Generate Minimap";
            this.generateMinimapMenuItem.Click += new System.EventHandler(this.GenerateMinimapMenuItemClick);
            // 
            // generateMinimapHighQualityMenuItem
            // 
            this.generateMinimapHighQualityMenuItem.Enabled = false;
            this.generateMinimapHighQualityMenuItem.Name = "generateMinimapHighQualityMenuItem";
            this.generateMinimapHighQualityMenuItem.Size = new System.Drawing.Size(373, 34);
            this.generateMinimapHighQualityMenuItem.Text = "Generate Minimap (High Quality)";
            this.generateMinimapHighQualityMenuItem.Click += new System.EventHandler(this.GenerateMinimapHighQualityMenuItemClick);
            // 
            // preferencesMenuItem
            // 
            this.preferencesMenuItem.Name = "preferencesMenuItem";
            this.preferencesMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.preferencesMenuItem.Size = new System.Drawing.Size(373, 34);
            this.preferencesMenuItem.Text = "&Preferences...";
            this.preferencesMenuItem.Click += new System.EventHandler(this.PreferencesMenuItemClick);
            // 
            // viewMenuItem
            // 
            this.viewMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toggleHeightmapMenuItem,
            this.toggleHeightGridMenuItem,
            this.toggleMinimapMenuItem,
            this.toggleVoidsMenuItem,
            this.gridMenuItem,
            this.toggleFeaturesMenuItem});
            this.viewMenuItem.Name = "viewMenuItem";
            this.viewMenuItem.Size = new System.Drawing.Size(65, 29);
            this.viewMenuItem.Text = "&View";
            // 
            // toggleHeightmapMenuItem
            // 
            this.toggleHeightmapMenuItem.Name = "toggleHeightmapMenuItem";
            this.toggleHeightmapMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.toggleHeightmapMenuItem.Size = new System.Drawing.Size(345, 34);
            this.toggleHeightmapMenuItem.Text = "&Heightmap Contours";
            this.toggleHeightmapMenuItem.Click += new System.EventHandler(this.ToggleHeightmapMenuItemClick);
            // 
            // toggleHeightGridMenuItem
            // 
            this.toggleHeightGridMenuItem.Name = "toggleHeightGridMenuItem";
            this.toggleHeightGridMenuItem.Size = new System.Drawing.Size(345, 34);
            this.toggleHeightGridMenuItem.Text = "Heightmap &Grid";
            this.toggleHeightGridMenuItem.Click += new System.EventHandler(this.ToggleHeightGridMenuItemClick);
            // 
            // toggleMinimapMenuItem
            // 
            this.toggleMinimapMenuItem.Name = "toggleMinimapMenuItem";
            this.toggleMinimapMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.toggleMinimapMenuItem.Size = new System.Drawing.Size(345, 34);
            this.toggleMinimapMenuItem.Text = "&Minimap";
            this.toggleMinimapMenuItem.Click += new System.EventHandler(this.ToggleMinimapMenuItemClick);
            // 
            // toggleVoidsMenuItem
            // 
            this.toggleVoidsMenuItem.Name = "toggleVoidsMenuItem";
            this.toggleVoidsMenuItem.Size = new System.Drawing.Size(345, 34);
            this.toggleVoidsMenuItem.Text = "Voids";
            this.toggleVoidsMenuItem.Click += new System.EventHandler(this.ToggleVoidsMenuItemClick);
            // 
            // gridMenuItem
            // 
            this.gridMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gridOffMenuItem,
            toolStripSeparator5,
            this.grid16MenuItem,
            this.grid32MenuItem,
            this.grid64MenuItem,
            this.grid128MenuItem,
            this.grid256MenuItem,
            this.grid512MenuItem,
            this.grid1024MenuItem,
            toolStripSeparator6,
            this.gridColorMenuItem});
            this.gridMenuItem.Name = "gridMenuItem";
            this.gridMenuItem.Size = new System.Drawing.Size(345, 34);
            this.gridMenuItem.Text = "Grid";
            // 
            // gridOffMenuItem
            // 
            this.gridOffMenuItem.Checked = true;
            this.gridOffMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.gridOffMenuItem.Name = "gridOffMenuItem";
            this.gridOffMenuItem.Size = new System.Drawing.Size(234, 34);
            this.gridOffMenuItem.Tag = "0";
            this.gridOffMenuItem.Text = "Off";
            this.gridOffMenuItem.Click += new System.EventHandler(this.GridOffMenuItemClick);
            // 
            // grid16MenuItem
            // 
            this.grid16MenuItem.Name = "grid16MenuItem";
            this.grid16MenuItem.Size = new System.Drawing.Size(234, 34);
            this.grid16MenuItem.Tag = "16";
            this.grid16MenuItem.Text = "16x16";
            this.grid16MenuItem.Click += new System.EventHandler(this.GridMenuItemClick);
            // 
            // grid32MenuItem
            // 
            this.grid32MenuItem.Name = "grid32MenuItem";
            this.grid32MenuItem.Size = new System.Drawing.Size(234, 34);
            this.grid32MenuItem.Tag = "32";
            this.grid32MenuItem.Text = "32x32";
            this.grid32MenuItem.Click += new System.EventHandler(this.GridMenuItemClick);
            // 
            // grid64MenuItem
            // 
            this.grid64MenuItem.Name = "grid64MenuItem";
            this.grid64MenuItem.Size = new System.Drawing.Size(234, 34);
            this.grid64MenuItem.Tag = "64";
            this.grid64MenuItem.Text = "64x64";
            this.grid64MenuItem.Click += new System.EventHandler(this.GridMenuItemClick);
            // 
            // grid128MenuItem
            // 
            this.grid128MenuItem.Name = "grid128MenuItem";
            this.grid128MenuItem.Size = new System.Drawing.Size(234, 34);
            this.grid128MenuItem.Tag = "128";
            this.grid128MenuItem.Text = "128x128";
            this.grid128MenuItem.Click += new System.EventHandler(this.GridMenuItemClick);
            // 
            // grid256MenuItem
            // 
            this.grid256MenuItem.Name = "grid256MenuItem";
            this.grid256MenuItem.Size = new System.Drawing.Size(234, 34);
            this.grid256MenuItem.Tag = "256";
            this.grid256MenuItem.Text = "256x256";
            this.grid256MenuItem.Click += new System.EventHandler(this.GridMenuItemClick);
            // 
            // grid512MenuItem
            // 
            this.grid512MenuItem.Name = "grid512MenuItem";
            this.grid512MenuItem.Size = new System.Drawing.Size(234, 34);
            this.grid512MenuItem.Tag = "512";
            this.grid512MenuItem.Text = "512x512";
            this.grid512MenuItem.Click += new System.EventHandler(this.GridMenuItemClick);
            // 
            // grid1024MenuItem
            // 
            this.grid1024MenuItem.Name = "grid1024MenuItem";
            this.grid1024MenuItem.Size = new System.Drawing.Size(234, 34);
            this.grid1024MenuItem.Tag = "1024";
            this.grid1024MenuItem.Text = "1024x1024";
            this.grid1024MenuItem.Click += new System.EventHandler(this.GridMenuItemClick);
            // 
            // gridColorMenuItem
            // 
            this.gridColorMenuItem.Name = "gridColorMenuItem";
            this.gridColorMenuItem.Size = new System.Drawing.Size(234, 34);
            this.gridColorMenuItem.Text = "Choose Color...";
            this.gridColorMenuItem.Click += new System.EventHandler(this.GridColorMenuItemClick);
            // 
            // toggleFeaturesMenuItem
            // 
            this.toggleFeaturesMenuItem.Checked = true;
            this.toggleFeaturesMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toggleFeaturesMenuItem.Name = "toggleFeaturesMenuItem";
            this.toggleFeaturesMenuItem.Size = new System.Drawing.Size(345, 34);
            this.toggleFeaturesMenuItem.Text = "Features";
            this.toggleFeaturesMenuItem.Click += new System.EventHandler(this.ToggleFeaturesMenuItemClick);
            // 
            // helpMenuItem
            // 
            this.helpMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutMenuItem});
            this.helpMenuItem.Name = "helpMenuItem";
            this.helpMenuItem.Size = new System.Drawing.Size(65, 29);
            this.helpMenuItem.Text = "&Help";
            // 
            // aboutMenuItem
            // 
            this.aboutMenuItem.Name = "aboutMenuItem";
            this.aboutMenuItem.Size = new System.Drawing.Size(176, 34);
            this.aboutMenuItem.Text = "&About...";
            this.aboutMenuItem.Click += new System.EventHandler(this.AboutMenuItemClick);
            // 
            // sidebarTabs
            // 
            this.sidebarTabs.Controls.Add(this.sectionsTab);
            this.sidebarTabs.Controls.Add(this.featuresTab);
            this.sidebarTabs.Controls.Add(this.startPositionsTab);
            this.sidebarTabs.Controls.Add(this.attributesTab);
            this.sidebarTabs.Dock = System.Windows.Forms.DockStyle.Left;
            this.sidebarTabs.Location = new System.Drawing.Point(0, 35);
            this.sidebarTabs.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.sidebarTabs.Name = "sidebarTabs";
            this.sidebarTabs.SelectedIndex = 0;
            this.sidebarTabs.Size = new System.Drawing.Size(322, 830);
            this.sidebarTabs.TabIndex = 4;
            this.sidebarTabs.SelectedIndexChanged += new System.EventHandler(this.GUITabs_SelectedIndexChanged);
            // 
            // sectionsTab
            // 
            this.sectionsTab.Controls.Add(this.sectionsView);
            this.sectionsTab.Location = new System.Drawing.Point(4, 29);
            this.sectionsTab.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.sectionsTab.Name = "sectionsTab";
            this.sectionsTab.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.sectionsTab.Size = new System.Drawing.Size(314, 797);
            this.sectionsTab.TabIndex = 0;
            this.sectionsTab.Text = "Sections";
            this.sectionsTab.UseVisualStyleBackColor = true;
            // 
            // sectionsView
            // 
            this.sectionsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sectionsView.ImageSize = new System.Drawing.Size(128, 128);
            this.sectionsView.Location = new System.Drawing.Point(4, 5);
            this.sectionsView.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.sectionsView.Name = "sectionsView";
            this.sectionsView.Size = new System.Drawing.Size(306, 787);
            this.sectionsView.TabIndex = 3;
            // 
            // featuresTab
            // 
            this.featuresTab.Controls.Add(this.featureView);
            this.featuresTab.Location = new System.Drawing.Point(4, 29);
            this.featuresTab.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.featuresTab.Name = "featuresTab";
            this.featuresTab.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.featuresTab.Size = new System.Drawing.Size(314, 797);
            this.featuresTab.TabIndex = 1;
            this.featuresTab.Text = "Features";
            this.featuresTab.UseVisualStyleBackColor = true;
            // 
            // featureView
            // 
            this.featureView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.featureView.ImageSize = new System.Drawing.Size(64, 64);
            this.featureView.Location = new System.Drawing.Point(4, 5);
            this.featureView.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.featureView.Name = "featureView";
            this.featureView.Size = new System.Drawing.Size(306, 787);
            this.featureView.TabIndex = 0;
            // 
            // startPositionsTab
            // 
            this.startPositionsTab.Controls.Add(this.startPositionsView1);
            this.startPositionsTab.Location = new System.Drawing.Point(4, 29);
            this.startPositionsTab.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.startPositionsTab.Name = "startPositionsTab";
            this.startPositionsTab.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.startPositionsTab.Size = new System.Drawing.Size(314, 797);
            this.startPositionsTab.TabIndex = 2;
            this.startPositionsTab.Text = "Starts";
            this.startPositionsTab.UseVisualStyleBackColor = true;
            // 
            // startPositionsView1
            // 
            this.startPositionsView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.startPositionsView1.Location = new System.Drawing.Point(4, 5);
            this.startPositionsView1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.startPositionsView1.Name = "startPositionsView1";
            this.startPositionsView1.Size = new System.Drawing.Size(306, 787);
            this.startPositionsView1.TabIndex = 0;
            // 
            // attributesTab
            // 
            this.attributesTab.Controls.Add(this.seaLevelValueLabel);
            this.attributesTab.Controls.Add(this.seaLevelLabel);
            this.attributesTab.Controls.Add(this.seaLevelTrackbar);
            this.attributesTab.Location = new System.Drawing.Point(4, 29);
            this.attributesTab.Name = "attributesTab";
            this.attributesTab.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.attributesTab.Size = new System.Drawing.Size(314, 795);
            this.attributesTab.TabIndex = 3;
            this.attributesTab.Text = "Attributes";
            this.attributesTab.UseVisualStyleBackColor = true;
            // 
            // seaLevelValueLabel
            // 
            this.seaLevelValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.seaLevelValueLabel.Enabled = false;
            this.seaLevelValueLabel.Location = new System.Drawing.Point(260, 8);
            this.seaLevelValueLabel.Name = "seaLevelValueLabel";
            this.seaLevelValueLabel.Size = new System.Drawing.Size(48, 22);
            this.seaLevelValueLabel.TabIndex = 10;
            this.seaLevelValueLabel.Text = "0";
            this.seaLevelValueLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // seaLevelLabel
            // 
            this.seaLevelLabel.AutoSize = true;
            this.seaLevelLabel.Enabled = false;
            this.seaLevelLabel.Location = new System.Drawing.Point(6, 8);
            this.seaLevelLabel.Name = "seaLevelLabel";
            this.seaLevelLabel.Size = new System.Drawing.Size(79, 20);
            this.seaLevelLabel.TabIndex = 9;
            this.seaLevelLabel.Text = "Sea Level";
            // 
            // seaLevelTrackbar
            // 
            this.seaLevelTrackbar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.seaLevelTrackbar.Enabled = false;
            this.seaLevelTrackbar.Location = new System.Drawing.Point(93, 8);
            this.seaLevelTrackbar.Maximum = 255;
            this.seaLevelTrackbar.Name = "seaLevelTrackbar";
            this.seaLevelTrackbar.Size = new System.Drawing.Size(159, 69);
            this.seaLevelTrackbar.TabIndex = 8;
            this.seaLevelTrackbar.TickFrequency = 16;
            this.seaLevelTrackbar.ValueChanged += new System.EventHandler(this.SeaLevelTrackBarValueChanged);
            this.seaLevelTrackbar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SeaLevelTrackBarMouseUp);
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mousePositionLabel,
            this.hoveredFeatureLabel});
            this.statusStrip.Location = new System.Drawing.Point(322, 833);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(2, 0, 21, 0);
            this.statusStrip.Size = new System.Drawing.Size(854, 32);
            this.statusStrip.TabIndex = 6;
            this.statusStrip.Text = "statusStrip1";
            // 
            // mousePositionLabel
            // 
            this.mousePositionLabel.Name = "mousePositionLabel";
            this.mousePositionLabel.Size = new System.Drawing.Size(74, 25);
            this.mousePositionLabel.Text = "X: -, Y: -";
            // 
            // hoveredFeatureLabel
            // 
            this.hoveredFeatureLabel.Name = "hoveredFeatureLabel";
            this.hoveredFeatureLabel.Size = new System.Drawing.Size(33, 25);
            this.hoveredFeatureLabel.Text = "---";
            // 
            // mapViewPanel
            // 
            this.mapViewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapViewPanel.Location = new System.Drawing.Point(322, 35);
            this.mapViewPanel.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.mapViewPanel.Name = "mapViewPanel";
            this.mapViewPanel.Size = new System.Drawing.Size(854, 798);
            this.mapViewPanel.TabIndex = 5;
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1176, 865);
            this.Controls.Add(this.mapViewPanel);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.sidebarTabs);
            this.Controls.Add(this.topMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.topMenu;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MainForm";
            this.Text = "Mappy";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormFormClosing);
            this.Load += new System.EventHandler(this.MainFormLoad);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainFormDragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainFormDragEnter);
            this.topMenu.ResumeLayout(false);
            this.topMenu.PerformLayout();
            this.sidebarTabs.ResumeLayout(false);
            this.sectionsTab.ResumeLayout(false);
            this.featuresTab.ResumeLayout(false);
            this.startPositionsTab.ResumeLayout(false);
            this.attributesTab.ResumeLayout(false);
            this.attributesTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seaLevelTrackbar)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip topMenu;
        private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toggleHeightmapMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editMenuItem;
        private System.Windows.Forms.ToolStripMenuItem preferencesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toggleMinimapMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoMenuItem;
        private SectionView sectionsView;
        private System.Windows.Forms.ToolStripMenuItem newMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateMinimapMenuItem;
        private System.Windows.Forms.TabControl sidebarTabs;
        private System.Windows.Forms.TabPage sectionsTab;
        private System.Windows.Forms.TabPage featuresTab;
        private SectionView featureView;
        private System.Windows.Forms.ToolStripMenuItem gridMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gridOffMenuItem;
        private System.Windows.Forms.ToolStripMenuItem grid16MenuItem;
        private System.Windows.Forms.ToolStripMenuItem grid32MenuItem;
        private System.Windows.Forms.ToolStripMenuItem grid64MenuItem;
        private System.Windows.Forms.ToolStripMenuItem grid128MenuItem;
        private System.Windows.Forms.ToolStripMenuItem grid256MenuItem;
        private System.Windows.Forms.ToolStripMenuItem grid512MenuItem;
        private System.Windows.Forms.ToolStripMenuItem grid1024MenuItem;
        private System.Windows.Forms.ToolStripMenuItem gridColorMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toggleFeaturesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mapAttributesMenuItem;
        private System.Windows.Forms.TabPage startPositionsTab;
        private StartPositionsView startPositionsView1;
        private System.Windows.Forms.TabPage attributesTab;
        private System.Windows.Forms.Label seaLevelValueLabel;
        private System.Windows.Forms.Label seaLevelLabel;
        private System.Windows.Forms.TrackBar seaLevelTrackbar;
        private System.Windows.Forms.ToolStripMenuItem closeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateMinimapHighQualityMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportMinimapMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportHeightmapMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importMinimapMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importHeightmapMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportMapImageMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importCustomSectionMenuItem;
        private MapViewPanel mapViewPanel;
        private System.Windows.Forms.ToolStripMenuItem toggleVoidsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toggleHeightGridMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel mousePositionLabel;
        private System.Windows.Forms.ToolStripStatusLabel hoveredFeatureLabel;
        private System.Windows.Forms.ToolStripMenuItem fillMenuItem;
    }
}

