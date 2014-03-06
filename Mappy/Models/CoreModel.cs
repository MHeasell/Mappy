namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using Data;

    using Mappy.Collections;
    using Mappy.IO;
    using Mappy.Models.Session;
    using Mappy.Presentation;

    using Operations;

    using TAUtil.Hpi;
    using TAUtil.Sct;
    using TAUtil.Tdf;
    using TAUtil.Tnt;

    using Util;

    public class CoreModel : Notifier, IMapDataModel, IMapPresenterCommandModel, IViewOptionsModel
    {
        private readonly OperationManager undoManager = new OperationManager();

        private readonly IFeatureDatabase featureRecords;
        private readonly IList<Section> sections;

        private readonly SectionFactory sectionFactory;

        private readonly MapModelFactory mapModelFactory;

        private readonly MapSaver mapSaver;

        private IBindingMapModel map;
        private bool isDirty;
        private string openFilePath;
        private bool isFileOpen;
        private bool isFileReadOnly;
        private bool heightmapVisible;
        private bool featuresVisible = true;

        private bool gridVisible;
        private Size gridSize = new Size(16, 16);
        private Color gridColor = MappySettings.Settings.GridColor;

        private bool previousTranslationOpen;

        public CoreModel()
        {
            this.featureRecords = new FeatureDictionary(Globals.Palette);
            this.sections = LoadingUtils.LoadSections(Globals.Palette);

            this.sectionFactory = new SectionFactory(Globals.Palette);
            this.mapModelFactory = new MapModelFactory(
                Globals.Palette,
                this.featureRecords,
                Mappy.Properties.Resources.nofeature);

            this.mapSaver = new MapSaver(Globals.ReversePalette);

            // hook up undoManager
            this.undoManager.CanUndoChanged += this.CanUndoChanged;
            this.undoManager.CanRedoChanged += this.CanRedoChanged;
            this.undoManager.IsMarkedChanged += this.IsMarkedChanged;
        }

        public IBindingMapModel Map
        {
            get
            {
                return this.map;
            }

            private set
            {
                if (this.SetField(ref this.map, value, "Map"))
                {
                    this.undoManager.Clear();
                    this.IsFileOpen = this.Map != null;
                    this.previousTranslationOpen = false;
                }
            }
        }

        public bool CanUndo
        {
            get { return this.undoManager.CanUndo; }
        }

        public bool CanRedo
        {
            get { return this.undoManager.CanRedo; }
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
            get { return this.isDirty; }
            private set { this.SetField(ref this.isDirty, value, "IsDirty"); }
        }

        public string FilePath
        {
            get { return this.openFilePath; }
            private set { this.SetField(ref this.openFilePath, value, "FilePath"); }
        }

        public bool IsFileOpen
        {
            get { return this.isFileOpen; }
            private set { this.SetField(ref this.isFileOpen, value, "IsFileOpen"); }
        }

        public bool IsFileReadOnly
        {
            get { return this.isFileReadOnly; }
            private set { this.SetField(ref this.isFileReadOnly, value, "IsFileReadOnly"); }
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

        public void ToggleHeightmap()
        {
            this.HeightmapVisible = !this.HeightmapVisible;
        }

        public void Undo()
        {
            this.undoManager.Undo();
        }

        public void Redo()
        {
            this.undoManager.Redo();
        }

        public void New(int width, int height)
        {
            this.Map = new BindingMapModel(new MapModel(width, height));
            this.FilePath = null;
            this.IsFileReadOnly = false;
        }

        public void SaveHpi(string filename)
        {
            // flatten before save --- only the base tile is written to disk
            IReplayableOperation flatten = OperationFactory.CreateFlattenOperation(this.Map);
            flatten.Execute();

            this.mapSaver.SaveHpi(this.Map, filename);

            flatten.Undo();

            this.undoManager.SetNowAsMark();

            this.FilePath = filename;
            this.IsFileReadOnly = false;
        }

        public void Save(string filename)
        {
            // flatten before save --- only the base tile is written to disk
            IReplayableOperation flatten = OperationFactory.CreateFlattenOperation(this.Map);
            flatten.Execute();

            this.mapSaver.SaveTnt(this.Map, filename);

            flatten.Undo();

            this.undoManager.SetNowAsMark();

            this.FilePath = filename;
            this.IsFileReadOnly = false;
        }

        public void OpenSct(string filename)
        {
            MapTile t;
            using (var s = new SctReader(filename))
            {
                t = this.sectionFactory.TileFromSct(s);
            }

            this.Map = new BindingMapModel(new MapModel(t));
        }

        public void OpenTnt(string filename)
        {
            MapModel m;
            using (var s = new TntReader(filename))
            {
                m = this.mapModelFactory.FromTnt(s);
            }

            this.Map = new BindingMapModel(m);
            this.FilePath = filename;
        }

        public void OpenHapi(string hpipath, string mappath, bool readOnly = false)
        {
            MapModel m;

            using (HpiReader hpi = new HpiReader(hpipath))
            {
                string otaPath = Path.ChangeExtension(mappath, ".ota");

                TdfNode n;

                using (StreamReader sr = new StreamReader(hpi.ReadTextFile(otaPath)))
                {
                    n = TdfNode.LoadTdf(sr);
                }

                using (var s = new TntReader(hpi.ReadFile(mappath)))
                {
                    m = this.mapModelFactory.FromTntAndOta(s, MapAttributes.Load(n));
                }
            }

            this.Map = new BindingMapModel(m);
            this.FilePath = hpipath;
            this.IsFileReadOnly = readOnly;
        }

        public int PlaceSection(int tileId, int x, int y)
        {
            if (this.Map == null)
            {
                return -1;
            }

            MapTile tile = this.sections[tileId].GetTile();

            this.undoManager.Execute(
                new AddFloatingTileOperation(
                    this.Map,
                    new Positioned<IMapTile>(tile, new Point(x, y))));

            return this.Map.FloatingTiles.Count - 1;
        }

        public void TranslateSection(int index, int x, int y)
        {
            this.TranslateSection(this.Map.FloatingTiles[index], x, y);
        }

        public bool TranslateFeature(int index, int x, int y)
        {
            var coords = this.Map.Features.ToCoords(index);
            return this.TranslateFeature(new Point(coords.X, coords.Y), x, y);
        }

        public bool TranslateFeature(Point featureCoord, int x, int y)
        {
            Point newCoord = new Point(
                featureCoord.X + x,
                featureCoord.Y + y);

            if (this.Map.Features.HasValue(newCoord.X, newCoord.Y))
            {
                return false;
            }

            MoveFeatureOperation newOp = new MoveFeatureOperation(
                this.Map.Features,
                featureCoord.X,
                featureCoord.Y,
                newCoord.X,
                newCoord.Y);

            MoveFeatureOperation lastOp = null;
            if (this.undoManager.CanUndo)
            {
                lastOp = this.undoManager.PeekUndo() as MoveFeatureOperation;
            }

            if (this.previousTranslationOpen && lastOp != null && lastOp.DestX == newOp.StartX && lastOp.DestY == newOp.StartY)
            {
                newOp.Execute();
                this.undoManager.Replace(lastOp.Combine(newOp));
            }
            else
            {
                this.undoManager.Execute(newOp);
            }

            this.previousTranslationOpen = true;

            return true;
        }

        public void FlushTranslation()
        {
            this.previousTranslationOpen = false;
        }

        public bool TryPlaceFeature(string name, int x, int y)
        {
            if (this.Map == null)
            {
                return false;
            }

            if (this.Map.Features.HasValue(x, y))
            {
                return false;
            }

            this.undoManager.Execute(
                new AddFeatureOperation(
                    this.Map.Features,
                    this.featureRecords[name],
                    x,
                    y));

            return true;
        }

        public void RefreshMinimap()
        {
            this.Map.Minimap = Util.GenerateMinimap(this.Map);
        }

        public void RemoveSection(int index)
        {
            this.undoManager.Execute(new RemoveTileOperation(this.Map.FloatingTiles, index));
        }

        public void RemoveFeature(int index)
        {
            var coords = this.Map.Features.ToCoords(index);
            this.RemoveFeature(coords.X, coords.Y);
        }

        public void RemoveFeature(int x, int y)
        {
            this.undoManager.Execute(new RemoveFeatureOperation(this.Map.Features, x, y));
        }

        public void RemoveFeature(Point coords)
        {
            this.RemoveFeature(coords.X, coords.Y);
        }

        public void SetStartPosition(int i, int x, int y)
        {
            if (this.Map == null)
            {
                return;
            }

            this.undoManager.Execute(new ChangeStartPositionOperation(this.Map, i, new Point(x, y)));
            this.previousTranslationOpen = false;
        }

        public void TranslateStartPosition(int i, int x, int y)
        {
            var startPos = this.Map.Attributes.GetStartPosition(i);

            if (startPos == null)
            {
                throw new ArgumentException("Start position " + i + " has not been placed");
            }

            this.TranslateStartPositionTo(i, startPos.Value.X + x, startPos.Value.Y + y);
        }

        public void RemoveStartPosition(int i)
        {
            this.undoManager.Execute(new RemoveStartPositionOperation(this.Map, i));
        }

        public void UpdateAttributes(MapAttributesResult newAttrs)
        {
            this.undoManager.Execute(new ChangeAttributesOperation(this.Map, newAttrs));
        }

        private void TranslateSection(Positioned<IMapTile> tile, int x, int y)
        {
            MoveTileOperation newOp = new MoveTileOperation(tile, x, y);

            MoveTileOperation lastOp = null;
            if (this.undoManager.CanUndo)
            {
                lastOp = this.undoManager.PeekUndo() as MoveTileOperation;
            }

            if (this.previousTranslationOpen && lastOp != null && lastOp.Tile == tile)
            {
                newOp.Execute();
                this.undoManager.Replace(lastOp.Combine(newOp));
            }
            else
            {
                this.undoManager.Execute(new MoveTileOperation(tile, x, y));
            }

            this.previousTranslationOpen = true;
        }

        private void TranslateStartPositionTo(int i, int x, int y)
        {
            var newOp = new ChangeStartPositionOperation(this.Map, i, new Point(x, y));

            ChangeStartPositionOperation lastOp = null;
            if (this.undoManager.CanUndo)
            {
                lastOp = this.undoManager.PeekUndo() as ChangeStartPositionOperation;
            }

            if (this.previousTranslationOpen && lastOp != null && lastOp.Index == i)
            {
                newOp.Execute();
                this.undoManager.Replace(lastOp.Combine(newOp));
            }
            else
            {
                this.undoManager.Execute(newOp);
            }

            this.previousTranslationOpen = true;
        }

        private void CanUndoChanged(object sender, EventArgs e)
        {
            this.FireChange("CanUndo");
        }

        private void CanRedoChanged(object sender, EventArgs e)
        {
            this.FireChange("CanRedo");
        }

        private void IsMarkedChanged(object sender, EventArgs e)
        {
            this.IsDirty = !this.undoManager.IsMarked;
        }
    }
}
