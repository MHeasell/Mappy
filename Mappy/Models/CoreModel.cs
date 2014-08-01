namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using Data;

    using Mappy.Collections;
    using Mappy.Database;
    using Mappy.IO;
    using Mappy.Minimap;
    using Mappy.Models.Session.BandboxBehaviours;
    using Mappy.Presentation;

    using Operations;

    using TAUtil.Hpi;
    using TAUtil.Sct;
    using TAUtil.Tdf;
    using TAUtil.Tnt;

    using Util;

    public class CoreModel : Notifier, IMinimapModel, ISelectionModel
    {
        private readonly OperationManager undoManager = new OperationManager();

        private readonly IFeatureDatabase featureRecords;
        private readonly IList<Section> sections;

        private readonly SectionFactory sectionFactory;

        private readonly MapModelFactory mapModelFactory;

        private readonly MapSaver mapSaver;

        private readonly ObservableCollection<GridCoordinates> selectedFeatures = new ObservableCollection<GridCoordinates>();

        private IBindingMapModel map;
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

        private int? selectedTile;

        private int? selectedStartPosition;

        private bool hasSelection;

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

            this.selectedFeatures.CollectionChanged += this.SelectedFeaturesChanged;
        }

        public event EventHandler<SparseGridEventArgs> FeaturesChanged;

        public event EventHandler<ListChangedEventArgs> TilesChanged;

        public event EventHandler<GridEventArgs> BaseTileGraphicsChanged;

        public event EventHandler<GridEventArgs> BaseTileHeightChanged;

        public event EventHandler<StartPositionChangedEventArgs> StartPositionChanged;

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
                    this.previousTranslationOpen = false;
                    
                    if (this.Map == null)
                    {
                        this.MinimapImage = null;
                        this.IsFileOpen = false;
                    }
                    else
                    {
                        this.MinimapImage = this.Map.Minimap;
                        this.Map.MinimapChanged += this.MapOnMinimapChanged;

                        this.Map.Features.EntriesChanged += this.FeaturesOnEntriesChanged;
                        this.Map.FloatingTiles.ListChanged += this.FloatingTilesOnListChanged;
                        this.Map.Tile.TileGridChanged += this.TileOnTileGridChanged;
                        this.Map.Tile.HeightGridChanged += this.TileOnHeightGridChanged;
                        this.Map.Attributes.StartPositionChanged += this.AttributesOnStartPositionChanged;
                        this.IsFileOpen = true;
                    }

                    this.FireChange("MapOpen");
                    this.FireChange("Features");
                    this.FireChange("FloatingTiles");
                    this.FireChange("BaseTile");
                    this.FireChange("MapWidth");
                    this.FireChange("MapHeight");

                    for (var i = 0; i < 10; i++)
                    {
                        this.AttributesOnStartPositionChanged(this, new StartPositionChangedEventArgs(i));
                    }
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

        public bool HasSelection
        {
            get
            {
                return this.hasSelection;
            }

            private set
            {
                this.SetField(ref this.hasSelection, value, "HasSelection");
            }
        }

        public int? SelectedTile
        {
            get
            {
                return this.selectedTile;
            }

            private set
            {
                if (this.SetField(ref this.selectedTile, value, "SelectedTile"))
                {
                    this.OnSelectionChanged();
                }
            }
        }

        public int? SelectedStartPosition
        {
            get
            {
                return this.selectedStartPosition;
            }

            private set
            {
                if (this.SetField(ref this.selectedStartPosition, value, "SelectedStartPosition"))
                {
                    this.OnSelectionChanged();
                }
            }
        }

        public Rectangle BandboxRectangle
        {
            get
            {
                return this.bandboxBehaviour.BandboxRectangle;
            }
        }

        public ICollection<GridCoordinates> SelectedFeatures
        {
            get
            {
                return this.selectedFeatures;
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

        public void DragDropStartPosition(int index, int x, int y)
        {
            this.SetStartPosition(index, x, y);
            this.SelectStartPosition(index);
        }

        public void DragDropTile(int id, int x, int y)
        {
            int quantX = x / 32;
            int quantY = y / 32;
            int index = this.PlaceSection(id, quantX, quantY);

            if (index != -1)
            {
                this.SelectTile(index);
            }
        }

        public void DragDropFeature(string name, int x, int y)
        {
            if (!this.MapOpen)
            {
                return;
            }

            Point? featurePos = this.ScreenToHeightIndex(x, y);
            if (featurePos.HasValue)
            {
                if (this.TryPlaceFeature(name, featurePos.Value.X, featurePos.Value.Y))
                {
                    this.SelectFeature(Util.ToGridCoordinates(featurePos.Value));
                }
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
                    var tmp = new List<GridCoordinates>(this.SelectedFeatures);
                    this.SelectedFeatures.Clear();

                    foreach (var item in tmp)
                    {
                        this.SelectedFeatures.Add(
                            new GridCoordinates(item.X + quantX, item.Y + quantY));
                    }

                    this.deltaX %= 16;
                    this.deltaY %= 16;
                }
            }

            this.previousTranslationOpen = true;
        }

        public void DeleteSelection()
        {
            if (this.SelectedFeatures.Count > 0)
            {
                foreach (var item in this.SelectedFeatures)
                {
                    this.RemoveFeature(item.X, item.Y);
                }

                this.SelectedFeatures.Clear();
            }
            else if (this.SelectedTile.HasValue)
            {
                this.RemoveSection(this.SelectedTile.Value);
                this.SelectedTile = null;
            }
            else if (this.SelectedStartPosition.HasValue)
            {
                this.RemoveStartPosition(this.SelectedStartPosition.Value);
                this.SelectedStartPosition = null;
            }
        }

        public Point? GetStartPosition(int index)
        {
            return this.Map.Attributes.GetStartPosition(index);
        }

        public void SelectTile(int index)
        {
            this.SelectedTile = index;
            this.SelectedFeatures.Clear();
            this.SelectedStartPosition = null;
        }

        public void SelectFeature(GridCoordinates index)
        {
            this.SelectedFeatures.Clear();
            this.SelectedFeatures.Add(index);
            this.MergeDownSelectedTile();
            this.SelectedStartPosition = null;
        }

        public void SelectStartPosition(int index)
        {
            this.MergeDownSelectedTile();
            this.SelectedFeatures.Clear();
            this.SelectedStartPosition = index;
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

        public void MergeSection(int index)
        {
            this.undoManager.Execute(OperationFactory.CreateMergeSectionOperation(this.Map, index));
        }

        public int LiftArea(int x, int y, int width, int height)
        {
            this.undoManager.Execute(OperationFactory.CreateClippedLiftAreaOperation(this.Map, x, y, width, height));
            return this.Map.FloatingTiles.Count - 1;
        }

        public Point? ScreenToHeightIndex(int x, int y)
        {
            return Util.ScreenToHeightIndex(this.Map.Tile.HeightGrid, new Point(x, y));
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

        public bool TranslateFeatureBatch(IEnumerable<GridCoordinates> coords, int x, int y)
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

        public void FlushTranslation()
        {
            this.previousTranslationOpen = false;
        }

        public void ClearSelection()
        {
            if (this.previousTranslationOpen)
            {
                this.FlushTranslation();
            }

            this.SelectedFeatures.Clear();
            this.MergeDownSelectedTile();
            this.SelectedStartPosition = null;
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

        private void MapOnMinimapChanged(object sender, EventArgs eventArgs)
        {
            this.MinimapImage = this.Map.Minimap;
        }

        private void TileOnHeightGridChanged(object sender, GridEventArgs gridEventArgs)
        {
            var h = this.BaseTileHeightChanged;
            if (h != null)
            {
                h(this, gridEventArgs);
            }
        }

        private void TileOnTileGridChanged(object sender, GridEventArgs gridEventArgs)
        {
            var h = this.BaseTileGraphicsChanged;
            if (h != null)
            {
                h(this, gridEventArgs);
            }
        }

        private void FloatingTilesOnListChanged(object sender, ListChangedEventArgs listChangedEventArgs)
        {
            var h = this.TilesChanged;
            if (h != null)
            {
                h(this, listChangedEventArgs);
            }
        }

        private void FeaturesOnEntriesChanged(object sender, SparseGridEventArgs sparseGridEventArgs)
        {
            var h = this.FeaturesChanged;
            if (h != null)
            {
                h(this, sparseGridEventArgs);
            }
        }

        private void AttributesOnStartPositionChanged(object sender, StartPositionChangedEventArgs startPositionChangedEventArgs)
        {
            var h = this.StartPositionChanged;
            if (h != null)
            {
                h(this, startPositionChangedEventArgs);
            }
        }

        private void MergeDownSelectedTile()
        {
            if (this.SelectedTile.HasValue)
            {
                this.MergeSection(this.SelectedTile.Value);
                this.SelectedTile = null;
            }
        }

        private void OnSelectionChanged()
        {
            this.HasSelection = this.SelectedFeatures.Count > 0
                    || this.SelectedTile.HasValue
                    || this.SelectedStartPosition.HasValue;
        }

        private void SelectedFeaturesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.FireChange("SelectedFeatures");
        }

        private void BandboxBehaviourPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "BandboxRectangle":
                    this.FireChange("BandboxRectangle");
                    break;
            }
        }
    }
}
