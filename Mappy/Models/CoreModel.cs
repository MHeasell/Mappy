namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Linq;

    using Data;

    using Mappy.Collections;
    using Mappy.Database;
    using Mappy.IO;
    using Mappy.Minimap;
    using Mappy.Models.BandboxBehaviours;
    using Mappy.Operations.SelectionModel;
    using Mappy.Util.ImageSampling;

    using Operations;

    using TAUtil.Hpi;
    using TAUtil.Sct;
    using TAUtil.Tdf;
    using TAUtil.Tnt;

    using Util;

    public class CoreModel : Notifier, IMinimapModel, IMainModel
    {
        private readonly OperationManager undoManager = new OperationManager();

        private readonly IFeatureDatabase featureRecords;
        private readonly IList<Section> sections;

        private readonly SectionFactory sectionFactory;

        private readonly MapModelFactory mapModelFactory;

        private readonly MapSaver mapSaver;

        private ISelectionModel map;
        private bool isDirty;
        private string openFilePath;
        private bool isFileOpen;
        private bool isFileReadOnly;
        private bool heightmapVisible;
        private bool featuresVisible = true;

        private bool minimapVisible;

        private bool gridVisible;
        private Size gridSize = new Size(16, 16);
        private Color gridColor = MappySettings.Settings.GridColor;

        private bool previousTranslationOpen;

        private BandboxMode selectionMode;

        private RectangleF viewportRectangle;

        private Bitmap minimapImage;

        private int deltaX;

        private int deltaY;

        private IBandboxBehaviour bandboxBehaviour;

        private bool previousSeaLevelOpen;

        public CoreModel()
        {
            this.bandboxBehaviour = new TileBandboxBehaviour(this);

            this.bandboxBehaviour.PropertyChanged += this.BandboxBehaviourPropertyChanged;

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

        public event EventHandler<SparseGridEventArgs> FeaturesChanged;

        public event EventHandler<ListChangedEventArgs> TilesChanged;

        public event EventHandler<GridEventArgs> BaseTileGraphicsChanged;

        public event EventHandler<GridEventArgs> BaseTileHeightChanged;

        public event EventHandler<StartPositionChangedEventArgs> StartPositionChanged;

        public ISelectionModel Map
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
                    this.previousTranslationOpen = false;
                    
                    if (this.Map == null)
                    {
                        this.MinimapImage = null;
                        this.IsFileOpen = false;
                    }
                    else
                    {
                        this.MinimapImage = this.Map.Minimap;

                        this.Map.SelectedStartPositionChanged += this.MapSelectedStartPositionChanged;
                        this.Map.SelectedTileChanged += this.MapSelectedTileChanged;
                        this.Map.SelectedFeatures.CollectionChanged += this.SelectedFeaturesChanged;

                        this.Map.MinimapChanged += this.MapOnMinimapChanged;

                        this.Map.Features.EntriesChanged += this.FeaturesOnEntriesChanged;
                        this.Map.FloatingTiles.ListChanged += this.FloatingTilesOnListChanged;
                        this.Map.Tile.TileGridChanged += this.TileOnTileGridChanged;
                        this.Map.Tile.HeightGridChanged += this.TileOnHeightGridChanged;
                        this.Map.Attributes.StartPositionChanged += this.AttributesOnStartPositionChanged;
                        this.Map.SeaLevelChanged += this.MapSeaLevelChanged;
                        this.IsFileOpen = true;
                    }

                    this.FireChange("MapOpen");
                    this.FireChange("Features");
                    this.FireChange("FloatingTiles");
                    this.FireChange("BaseTile");
                    this.FireChange("MapWidth");
                    this.FireChange("MapHeight");
                    this.FireChange("SeaLevel");

                    this.FireChange("SelectedTile");
                    this.FireChange("SelectedStartPosition");
                    this.FireChange("SelectedFeatures");

                    for (var i = 0; i < 10; i++)
                    {
                        this.AttributesOnStartPositionChanged(this, new StartPositionChangedEventArgs(i));
                    }
                }
            }
        }

        public int SeaLevel
        {
            get
            {
                return this.Map == null ? 0 : this.Map.SeaLevel;
            }

            set
            {
                this.Map.SeaLevel = value;
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

        public int? SelectedTile
        {
            get
            {
                return this.Map == null ? null : this.Map.SelectedTile;
            }
        }

        public int? SelectedStartPosition
        {
            get
            {
                return this.Map == null ? null : this.Map.SelectedStartPosition;
            }
        }

        public ICollection<GridCoordinates> SelectedFeatures
        {
            get
            {
                return this.Map == null ? null : this.Map.SelectedFeatures;
            }
        }

        public Rectangle BandboxRectangle
        {
            get
            {
                return this.bandboxBehaviour.BandboxRectangle;
            }
        }

        public ISparseGrid<Feature> Features
        {
            get
            {
                return this.Map == null ? null : this.Map.Features;
            }
        }

        public IList<Positioned<IMapTile>> FloatingTiles
        {
            get
            {
                return this.Map == null ? null : this.Map.FloatingTiles;
            }
        }

        public IMapTile BaseTile
        {
            get
            {
                return this.Map == null ? null : this.Map.Tile;
            }
        }

        public int MapWidth
        {
            get
            {
                return this.Map == null ? 0 : this.Map.Tile.TileGrid.Width;
            }
        }

        public int MapHeight
        {
            get
            {
                return this.Map == null ? 0 : this.Map.Tile.TileGrid.Height;
            }
        }

        public bool MapOpen
        {
            get
            {
                return this.Map != null;
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

        public BandboxMode SelectionMode
        {
            get
            {
                return this.selectionMode;
            }

            set
            {
                this.SetField(ref this.selectionMode, value, "SelectionMode");
            }
        }

        public RectangleF ViewportRectangle
        {
            get
            {
                return this.viewportRectangle;
            }

            set
            {
                this.SetField(ref this.viewportRectangle, value, "ViewportRectangle");
            }
        }

        public Bitmap MinimapImage
        {
            get
            {
                return this.minimapImage;
            }

            private set
            {
                this.SetField(ref this.minimapImage, value, "MinimapImage");
            }
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
            var map = new SelectionMapModel(new BindingMapModel(new MapModel(width, height)));
            GridMethods.Fill(map.Tile.TileGrid, Globals.DefaultTile);
            this.Map = map;
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

            var otaName = filename.Substring(0, filename.Length - 4) + ".ota";
            this.mapSaver.SaveTnt(this.Map, filename);
            this.mapSaver.SaveOta(this.Map.Attributes, otaName);

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

            this.Map = new SelectionMapModel(new BindingMapModel(new MapModel(t)));
        }

        public void OpenTnt(string filename)
        {
            MapModel m;

            var otaFileName = filename.Substring(0, filename.Length - 4) + ".ota";
            if (File.Exists(otaFileName))
            {
                TdfNode attrs;
                using (var ota = new StreamReader(File.OpenRead(otaFileName)))
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

            this.Map = new SelectionMapModel(new BindingMapModel(m));
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
                    m = this.mapModelFactory.FromTntAndOta(s, n);
                }
            }

            this.Map = new SelectionMapModel(new BindingMapModel(m));
            this.FilePath = hpipath;
            this.IsFileReadOnly = readOnly;
        }

        public void DragDropStartPosition(int index, int x, int y)
        {
            if (this.Map == null)
            {
                return;
            }

            var location = new Point(x, y);

            var op = new CompositeOperation(
                OperationFactory.CreateDeselectAndMergeOperation(this.Map),
                new ChangeStartPositionOperation(this.Map, index, location),
                new SelectStartPositionOperation(this.Map, index));

            this.undoManager.Execute(op);
            this.previousTranslationOpen = false;
        }

        public void DragDropTile(int id, int x, int y)
        {
            if (this.Map == null)
            {
                return;
            }

            int quantX = x / 32;
            int quantY = y / 32;

            var location = new Point(quantX, quantY);
            var section = this.Sections[id].GetTile();

            var floatingSection = new Positioned<IMapTile>(section, location);

            var addOp = new AddFloatingTileOperation(this.Map, floatingSection);

            // Tile's index should always be 0,
            // because all other tiles are merged before adding this one.
            var index = 0;

            var selectOp = new SelectTileOperation(this.Map, index);

            var op = new CompositeOperation(
                OperationFactory.CreateDeselectAndMergeOperation(this.Map),
                addOp,
                selectOp);

            this.undoManager.Execute(op);
        }

        public void DragDropFeature(string name, int x, int y)
        {
            if (this.Map == null)
            {
                return;
            }

            Point? featurePos = this.ScreenToHeightIndex(x, y);
            if (featurePos.HasValue && !this.Map.Features.HasValue(featurePos.Value.X, featurePos.Value.Y))
            {
                var feature = this.featureRecords[name];
                var addOp = new AddFeatureOperation(this.Map.Features, feature, featurePos.Value.X, featurePos.Value.Y);
                var selectOp = new SelectFeatureOperation(
                    this.Map,
                    new GridCoordinates(featurePos.Value.X, featurePos.Value.Y));
                var op = new CompositeOperation(
                    OperationFactory.CreateDeselectAndMergeOperation(this.Map),
                    addOp,
                    selectOp);
                this.undoManager.Execute(op);
            }
        }

        public void StartBandbox(int x, int y)
        {
            this.bandboxBehaviour.StartBandbox(x, y);
        }

        public void GrowBandbox(int x, int y)
        {
            this.bandboxBehaviour.GrowBandbox(x, y);
        }

        public void CommitBandbox()
        {
            this.bandboxBehaviour.CommitBandbox();
        }

        public void TranslateSelection(int x, int y)
        {
            if (this.SelectedStartPosition.HasValue)
            {
                this.TranslateStartPosition(
                    this.SelectedStartPosition.Value,
                    x,
                    y);
            }
            else if (this.SelectedTile.HasValue)
            {
                this.deltaX += x;
                this.deltaY += y;

                this.TranslateSection(
                    this.SelectedTile.Value,
                    this.deltaX / 32,
                    this.deltaY / 32);

                this.deltaX %= 32;
                this.deltaY %= 32;
            }
            else if (this.SelectedFeatures.Count > 0)
            {
                // TODO: restore old behaviour
                // where heightmap is taken into account when placing features

                this.deltaX += x;
                this.deltaY += y;

                int quantX = this.deltaX / 16;
                int quantY = this.deltaY / 16;

                bool success = this.TranslateFeatureBatch(
                    this.SelectedFeatures,
                    quantX,
                    quantY);

                if (success)
                {
                    this.deltaX %= 16;
                    this.deltaY %= 16;
                }
            }
        }

        public void DeleteSelection()
        {
            if (this.SelectedFeatures.Count > 0)
            {
                var ops = new List<IReplayableOperation>();
                ops.Add(new DeselectOperation(this.Map));
                ops.AddRange(this.SelectedFeatures.Select(x => new RemoveFeatureOperation(this.Map.Features, x.X, x.Y)));
                this.undoManager.Execute(new CompositeOperation(ops));
            }

            if (this.SelectedTile.HasValue)
            {
                var deSelectOp = new DeselectOperation(this.Map);
                var removeOp = new RemoveTileOperation(this.Map.FloatingTiles, this.SelectedTile.Value);
                this.undoManager.Execute(new CompositeOperation(deSelectOp, removeOp));
            }

            if (this.SelectedStartPosition.HasValue)
            {
                var deSelectOp = new DeselectOperation(this.Map);
                var removeOp = new RemoveStartPositionOperation(this.Map, this.SelectedStartPosition.Value);
                this.undoManager.Execute(new CompositeOperation(deSelectOp, removeOp));
            }
        }

        public Point? GetStartPosition(int index)
        {
            return this.Map == null ? null : this.Map.Attributes.GetStartPosition(index);
        }

        public void SelectTile(int index)
        {
            this.undoManager.Execute(
                new CompositeOperation(
                    OperationFactory.CreateDeselectAndMergeOperation(this.Map),
                    new SelectTileOperation(this.Map, index)));
        }

        public void SelectFeature(GridCoordinates index)
        {
            this.undoManager.Execute(
                new CompositeOperation(
                    OperationFactory.CreateDeselectAndMergeOperation(this.Map),
                    new SelectFeatureOperation(this.Map, index)));
        }

        public void SelectFeatures(IEnumerable<GridCoordinates> indices)
        {
            var list = new List<IReplayableOperation>();

            list.Add(OperationFactory.CreateDeselectAndMergeOperation(this.Map));
            list.AddRange(indices.Select(x => new SelectFeatureOperation(this.Map, x)));

            this.undoManager.Execute(new CompositeOperation(list));
        }

        public void SelectStartPosition(int index)
        {
            this.undoManager.Execute(
                new CompositeOperation(
                    OperationFactory.CreateDeselectAndMergeOperation(this.Map),
                    new SelectStartPositionOperation(this.Map, index)));
        }

        public void LiftAndSelectArea(int x, int y, int width, int height)
        {
            var liftOp = OperationFactory.CreateClippedLiftAreaOperation(this.Map, x, y, width, height);
            var index = this.Map.FloatingTiles.Count;
            var selectOp = new SelectTileOperation(this.Map, index);
            this.undoManager.Execute(new CompositeOperation(liftOp, selectOp));
        }

        public void FlushTranslation()
        {
            this.previousTranslationOpen = false;
            this.deltaX = 0;
            this.deltaY = 0;
        }

        public void ClearSelection()
        {
            if (this.SelectedTile == null && (this.SelectedFeatures == null || this.SelectedFeatures.Count == 0) && this.SelectedStartPosition == null)
            {
                return;
            }

            if (this.previousTranslationOpen)
            {
                this.FlushTranslation();
            }

            var deselectOp = new DeselectOperation(this.Map);

            if (this.Map.SelectedTile.HasValue)
            {
                var mergeOp = OperationFactory.CreateMergeSectionOperation(this.Map, this.Map.SelectedTile.Value);
                this.undoManager.Execute(new CompositeOperation(deselectOp, mergeOp));
            }
            else
            {
                this.undoManager.Execute(deselectOp);
            }
        }

        public void RefreshMinimap()
        {
            Bitmap minimap;
            using (var adapter = new MapPixelImageAdapter(this.Map.Tile.TileGrid))
            {
                minimap = Util.GenerateMinimap(adapter);
            }

            var op = new UpdateMinimapOperation(this.Map, minimap);
            this.undoManager.Execute(op);
        }

        public void RefreshMinimapHighQuality()
        {
            Bitmap minimap;
            using (var adapter = new MapPixelImageAdapter(this.Map.Tile.TileGrid))
            {
                minimap = Util.GenerateMinimapLinear(adapter);
            }

            var op = new UpdateMinimapOperation(this.Map, minimap);
            this.undoManager.Execute(op);
        }

        public void UpdateAttributes(MapAttributesResult newAttrs)
        {
            this.undoManager.Execute(new ChangeAttributesOperation(this.Map, newAttrs));
        }

        public MapAttributesResult GetAttributes()
        {
            return MapAttributesResult.FromModel(this.Map);
        }

        public void CloseMap()
        {
            this.Map = null;
        }

        public void SetSeaLevel(int value)
        {
            if (this.SeaLevel == value)
            {
                return;
            }

            var op = new SetSealevelOperation(this.Map, value);

            SetSealevelOperation prevOp = null;
            if (this.undoManager.CanUndo && this.previousSeaLevelOpen)
            {
                prevOp = this.undoManager.PeekUndo() as SetSealevelOperation;
            }

            if (prevOp == null)
            {
                this.undoManager.Execute(op);
            }
            else
            {
                op.Execute();
                var combinedOp = prevOp.Combine(op);
                this.undoManager.Replace(combinedOp);
            }

            this.previousSeaLevelOpen = true;
        }

        public void FlushSeaLevel()
        {
            this.previousSeaLevelOpen = false;
        }

        private Point? ScreenToHeightIndex(int x, int y)
        {
            return Util.ScreenToHeightIndex(this.Map.Tile.HeightGrid, new Point(x, y));
        }

        private void TranslateSection(int index, int x, int y)
        {
            this.TranslateSection(this.Map.FloatingTiles[index], x, y);
        }

        private void TranslateSection(Positioned<IMapTile> tile, int x, int y)
        {
            if (x == 0 && y == 0)
            {
                return;
            }

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

        private bool TranslateFeature(int index, int x, int y)
        {
            var coords = this.Map.Features.ToCoords(index);
            return this.TranslateFeature(new Point(coords.X, coords.Y), x, y);
        }

        private bool TranslateFeature(Point featureCoord, int x, int y)
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

        private bool TranslateFeatureBatch(IEnumerable<GridCoordinates> coords, int x, int y)
        {
            if (x == 0 && y == 0)
            {
                return true;
            }

            var coordSet = new HashSet<GridCoordinates>(coords);

            // pre-move check to see if anything is in our way
            foreach (var item in coordSet)
            {
                var translatedPoint = new GridCoordinates(item.X + x, item.Y + y);

                if (translatedPoint.X < 0
                    || translatedPoint.Y < 0
                    || translatedPoint.X >= this.Map.Features.Width
                    || translatedPoint.Y >= this.Map.Features.Height)
                {
                    return false;
                }

                bool isBlocked = !coordSet.Contains(translatedPoint)
                    && this.Map.Features.HasValue(translatedPoint.X, translatedPoint.Y);
                if (isBlocked)
                {
                    return false;
                }
            }

            var newOp = new BatchMoveFeatureOperation(this.Map.Features, coordSet, x, y);

            BatchMoveFeatureOperation lastOp = null;
            if (this.undoManager.CanUndo)
            {
                lastOp = this.undoManager.PeekUndo() as BatchMoveFeatureOperation;
            }

            if (this.previousTranslationOpen && lastOp != null && lastOp.GetTranslatedCoords().SetEquals(coordSet))
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

        private void TranslateStartPosition(int i, int x, int y)
        {
            var startPos = this.Map.Attributes.GetStartPosition(i);

            if (startPos == null)
            {
                throw new ArgumentException("Start position " + i + " has not been placed");
            }

            this.TranslateStartPositionTo(i, startPos.Value.X + x, startPos.Value.Y + y);
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

        private void MapOnMinimapChanged(object sender, EventArgs e)
        {
            this.MinimapImage = this.Map.Minimap;
        }

        private void TileOnHeightGridChanged(object sender, GridEventArgs e)
        {
            var h = this.BaseTileHeightChanged;
            if (h != null)
            {
                h(this, e);
            }
        }

        private void TileOnTileGridChanged(object sender, GridEventArgs e)
        {
            var h = this.BaseTileGraphicsChanged;
            if (h != null)
            {
                h(this, e);
            }
        }

        private void FloatingTilesOnListChanged(object sender, ListChangedEventArgs e)
        {
            var h = this.TilesChanged;
            if (h != null)
            {
                h(this, e);
            }
        }

        private void FeaturesOnEntriesChanged(object sender, SparseGridEventArgs e)
        {
            var h = this.FeaturesChanged;
            if (h != null)
            {
                h(this, e);
            }
        }

        private void AttributesOnStartPositionChanged(object sender, StartPositionChangedEventArgs e)
        {
            var h = this.StartPositionChanged;
            if (h != null)
            {
                h(this, e);
            }
        }

        private void SelectedFeaturesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.FireChange("SelectedFeatures");
        }

        private void BandboxBehaviourPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "BandboxRectangle":
                    this.FireChange("BandboxRectangle");
                    break;
            }
        }

        private void MapSelectedTileChanged(object sender, EventArgs eventArgs)
        {
            this.FireChange("SelectedTile");
        }

        private void MapSelectedStartPositionChanged(object sender, EventArgs eventArgs)
        {
            this.FireChange("SelectedStartPosition");
        }

        private void MapSeaLevelChanged(object sender, EventArgs e)
        {
            this.FireChange("SeaLevel");
        }
    }
}
