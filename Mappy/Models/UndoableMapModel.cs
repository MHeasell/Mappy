namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    using Mappy.Collections;
    using Mappy.Data;
    using Mappy.IO;
    using Mappy.Models.BandboxBehaviours;
    using Mappy.Operations;
    using Mappy.Operations.SelectionModel;
    using Mappy.Util;
    using Mappy.Util.ImageSampling;

    public class UndoableMapModel : Notifier, IMainModel, IBandboxModel
    {
        private readonly OperationManager undoManager = new OperationManager();

        private readonly IDialogService dialogService;

        private readonly MapSaver mapSaver;

        private readonly IBandboxBehaviour bandboxBehaviour;

        private readonly ISelectionModel model;

        private Point viewportLocation;

        private int deltaX;

        private int deltaY;

        private bool previousTranslationOpen;

        private bool previousSeaLevelOpen;

        private string openFilePath;

        private bool isFileReadOnly;

        private bool canCut;

        private bool canCopy;

        public UndoableMapModel(ISelectionModel model, IDialogService svc, string path, bool readOnly)
        {
            this.FilePath = path;
            this.IsFileReadOnly = readOnly;

            this.mapSaver = new MapSaver();

            this.dialogService = svc;

            this.model = model;

            model.PropertyChanged += this.ModelOnPropertyChanged;

            model.FloatingTilesChanged += this.FloatingTilesOnListChanged;

            model.TileGridChanged += this.TileOnTileGridChanged;

            model.HeightGridChanged += this.TileOnHeightGridChanged;
            model.Attributes.StartPositionChanged += this.AttributesOnStartPositionChanged;

            this.bandboxBehaviour = new TileBandboxBehaviour(this);
            this.bandboxBehaviour.PropertyChanged += this.BandboxBehaviourPropertyChanged;

            this.undoManager.CanUndoChanged += this.UndoManagerOnCanUndoChanged;
            this.undoManager.CanRedoChanged += this.UndoManagerOnCanRedoChanged;
            this.undoManager.IsMarkedChanged += this.UndoManagerOnIsMarkedChanged;

            this.model.SelectedFeatures.CollectionChanged += this.SelectedFeaturesCollectionChanged;
        }

        public event EventHandler<ListChangedEventArgs> TilesChanged;

        public event EventHandler<GridEventArgs> BaseTileGraphicsChanged;

        public event EventHandler<GridEventArgs> BaseTileHeightChanged;

        public event EventHandler<StartPositionChangedEventArgs> StartPositionChanged;

        public event EventHandler<FeatureInstanceEventArgs> FeatureInstanceChanged
        {
            add
            {
                this.model.FeatureInstanceChanged += value;
            }

            remove
            {
                this.model.FeatureInstanceChanged -= value;
            }
        }

        public bool CanCopy
        {
            get
            {
                return this.canCopy;
            }

            set
            {
                this.SetField(ref this.canCopy, value, "CanCopy");
            }
        }

        public bool CanCut
        {
            get
            {
                return this.canCut;
            }

            set
            {
                this.SetField(ref this.canCut, value, "CanCopy");
            }
        }

        public string FilePath
        {
            get { return this.openFilePath; }
            private set { this.SetField(ref this.openFilePath, value, "FilePath"); }
        }

        public bool IsFileReadOnly
        {
            get { return this.isFileReadOnly; }
            private set { this.SetField(ref this.isFileReadOnly, value, "IsFileReadOnly"); }
        }

        public bool CanUndo => this.undoManager.CanUndo;

        public bool CanRedo => this.undoManager.CanRedo;

        public bool IsMarked => this.undoManager.IsMarked;

        public Bitmap Minimap => this.model.Minimap;

        public int SeaLevel => this.model.SeaLevel;

        public Rectangle BandboxRectangle => this.bandboxBehaviour.BandboxRectangle;

        public IMapTile BaseTile => this.model.Tile;

        public int MapWidth => this.model.Tile.TileGrid.Width;

        public int MapHeight => this.model.Tile.TileGrid.Height;

        public int FeatureGridWidth => this.model.FeatureGridWidth;

        public int FeatureGridHeight => this.model.FeatureGridHeight;

        public IList<Positioned<IMapTile>> FloatingTiles => this.model.FloatingTiles;

        public int? SelectedStartPosition => this.model.SelectedStartPosition;

        public Point ViewportLocation
        {
            get
            {
                return this.viewportLocation;
            }

            set
            {
                this.SetField(ref this.viewportLocation, value, "ViewportLocation");
            }
        }

        public ObservableCollection<Guid> SelectedFeatures => this.model.SelectedFeatures;

        public int? SelectedTile => this.model.SelectedTile;

        public bool Save()
        {
            if (this.FilePath == null || this.IsFileReadOnly)
            {
                return this.SaveAs();
            }

            return this.SaveHelper(this.FilePath);
        }

        public bool SaveAs()
        {
            string path = this.dialogService.AskUserToSaveFile();

            if (path == null)
            {
                return false;
            }

            return this.SaveHelper(path);
        }

        public void Undo()
        {
            this.undoManager.Undo();
        }

        public void Redo()
        {
            this.undoManager.Redo();
        }

        public void AddFeatureInstance(FeatureInstance instance)
        {
            this.model.AddFeatureInstance(instance);
        }

        public FeatureInstance GetFeatureInstance(Guid id)
        {
            return this.model.GetFeatureInstance(id);
        }

        public FeatureInstance GetFeatureInstanceAt(int x, int y)
        {
            return this.model.GetFeatureInstanceAt(x, y);
        }

        public void RemoveFeatureInstance(Guid id)
        {
            this.model.RemoveFeatureInstance(id);
        }

        public bool HasFeatureInstanceAt(int x, int y)
        {
            return this.model.HasFeatureInstanceAt(x, y);
        }

        public void UpdateFeatureInstance(FeatureInstance instance)
        {
            this.model.UpdateFeatureInstance(instance);
        }

        public void SelectTile(int index)
        {
            this.undoManager.Execute(
                new CompositeOperation(
                    OperationFactory.CreateDeselectAndMergeOperation(this.model),
                    new SelectTileOperation(this.model, index)));
        }

        public void OpenMapAttributes()
        {
            MapAttributesResult r = this.dialogService.AskUserForMapAttributes(this.GetAttributes());

            if (r != null)
            {
                this.UpdateAttributes(r);
            }
        }

        public void SelectFeature(Guid id)
        {
            this.undoManager.Execute(
                new CompositeOperation(
                    OperationFactory.CreateDeselectAndMergeOperation(this.model),
                    new SelectFeatureOperation(this.model, id)));
        }

        public void SelectStartPosition(int index)
        {
            this.undoManager.Execute(new CompositeOperation(
                OperationFactory.CreateDeselectAndMergeOperation(this.model),
                new SelectStartPositionOperation(this.model, index)));
        }

        public void ExportHeightmap()
        {
            var loc = this.dialogService.AskUserToSaveHeightmap();
            if (loc == null)
            {
                return;
            }

            try
            {
                var b = Mappy.Util.Util.ExportHeightmap(this.model.Tile.HeightGrid);
                using (var s = File.Create(loc))
                {
                    b.Save(s, ImageFormat.Png);
                }
            }
            catch (Exception)
            {
                this.dialogService.ShowError("There was a problem saving the heightmap.");
            }
        }

        public void RefreshMinimap()
        {
            Bitmap minimap;
            using (var adapter = new MapPixelImageAdapter(this.model.Tile.TileGrid))
            {
                minimap = Util.GenerateMinimap(adapter);
            }

            var op = new UpdateMinimapOperation(this.model, minimap);
            this.undoManager.Execute(op);
        }

        public void RefreshMinimapHighQualityWithProgress()
        {
            var worker = Mappy.Util.Util.RenderMinimapWorker();

            var dlg = this.dialogService.CreateProgressView();
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
                    this.SetMinimap(img);
                }

                dlg.Close();
            };

            worker.RunWorkerAsync(this.model);
            dlg.Display();
        }

        public void DragDropStartPosition(int index, int x, int y)
        {
            var location = new Point(x, y);

            var op = new CompositeOperation(
                OperationFactory.CreateDeselectAndMergeOperation(this.model),
                new ChangeStartPositionOperation(this.model, index, location),
                new SelectStartPositionOperation(this.model, index));

            this.undoManager.Execute(op);
            this.previousTranslationOpen = false;
        }

        public void DragDropTile(IMapTile tile, int x, int y)
        {
            int quantX = x / 32;
            int quantY = y / 32;

            this.AddAndSelectTile(tile, quantX, quantY);
        }

        public void SetSeaLevel(int value)
        {
            if (this.SeaLevel == value)
            {
                return;
            }

            var op = new SetSealevelOperation(this.model, value);

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

        public IEnumerable<FeatureInstance> EnumerateFeatureInstances()
        {
            return this.model.EnumerateFeatureInstances();
        }

        public void CopySelectionToClipboard()
        {
            this.TryCopyToClipboard();
        }

        public void CutSelectionToClipboard()
        {
            if (this.TryCopyToClipboard())
            {
                this.DeleteSelection();
            }
        }

        public void PasteFromClipboard(int x, int y)
        {
            var data = Clipboard.GetData(DataFormats.Serializable);
            if (data == null)
            {
                return;
            }

            var tile = data as IMapTile;
            if (tile != null)
            {
                this.PasteMapTile(tile, x, y);
            }
            else
            {
                var record = data as FeatureClipboardRecord;
                if (record != null)
                {
                    this.PasteFeature(record, x, y);
                }
            }
        }

        public void DragDropFeature(string name, int x, int y)
        {
            Point? featurePos = this.ScreenToHeightIndex(x, y);
            if (featurePos.HasValue && !this.HasFeatureInstanceAt(featurePos.Value.X, featurePos.Value.Y))
            {
                var inst = new FeatureInstance(Guid.NewGuid(), name, featurePos.Value.X, featurePos.Value.Y);
                var addOp = new AddFeatureOperation(this.model, inst);
                var selectOp = new SelectFeatureOperation(this.model, inst.Id);
                var op = new CompositeOperation(
                    OperationFactory.CreateDeselectAndMergeOperation(this.model),
                    addOp,
                    selectOp);
                this.undoManager.Execute(op);
            }
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

            var deselectOp = new DeselectOperation(this.model);

            if (this.SelectedTile.HasValue)
            {
                var mergeOp = OperationFactory.CreateMergeSectionOperation(this.model, this.SelectedTile.Value);
                this.undoManager.Execute(new CompositeOperation(deselectOp, mergeOp));
            }
            else
            {
                this.undoManager.Execute(deselectOp);
            }
        }

        public void DeleteSelection()
        {
            if (this.SelectedFeatures.Count > 0)
            {
                var ops = new List<IReplayableOperation>();
                ops.Add(new DeselectOperation(this.model));
                ops.AddRange(this.SelectedFeatures.Select(x => new RemoveFeatureOperation(this.model, x)));
                this.undoManager.Execute(new CompositeOperation(ops));
            }

            if (this.SelectedTile.HasValue)
            {
                var deSelectOp = new DeselectOperation(this.model);
                var removeOp = new RemoveTileOperation(this.FloatingTiles, this.SelectedTile.Value);
                this.undoManager.Execute(new CompositeOperation(deSelectOp, removeOp));
            }

            if (this.SelectedStartPosition.HasValue)
            {
                var deSelectOp = new DeselectOperation(this.model);
                var removeOp = new RemoveStartPositionOperation(this.model, this.SelectedStartPosition.Value);
                this.undoManager.Execute(new CompositeOperation(deSelectOp, removeOp));
            }
        }

        public Point? GetStartPosition(int index)
        {
            return this.model.Attributes.GetStartPosition(index);
        }

        public void LiftAndSelectArea(int x, int y, int width, int height)
        {
            var liftOp = OperationFactory.CreateClippedLiftAreaOperation(this.model, x, y, width, height);
            var index = this.FloatingTiles.Count;
            var selectOp = new SelectTileOperation(this.model, index);
            this.undoManager.Execute(new CompositeOperation(liftOp, selectOp));
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

        public void ReplaceHeightmap(Grid<int> heightmap)
        {
            if (heightmap.Width != this.model.Tile.HeightGrid.Width
                || heightmap.Height != this.model.Tile.HeightGrid.Height)
            {
                throw new ArgumentException(
                    "Dimensions do not match map heightmap",
                    nameof(heightmap));
            }

            var op = new CopyAreaOperation<int>(
                heightmap,
                this.model.Tile.HeightGrid,
                0,
                0,
                0,
                0,
                heightmap.Width,
                heightmap.Height);
            this.undoManager.Execute(op);
        }

        public void SetMinimap(Bitmap minimap)
        {
            var op = new UpdateMinimapOperation(this.model, minimap);
            this.undoManager.Execute(op);
        }

        public void PasteMapTileNoDeduplicateTopLeft(IMapTile tile)
        {
            int x = this.ViewportLocation.X;
            int y = this.ViewportLocation.Y;

            this.AddAndSelectTile(tile, x, y);
        }

        private static void DeduplicateTiles(IGrid<Bitmap> tiles)
        {
            var len = tiles.Width * tiles.Height;
            for (int i = 0; i < len; i++)
            {
                tiles[i] = Globals.TileCache.GetOrAddBitmap(tiles[i]);
            }
        }

        private bool SaveHelper(string filename)
        {
            if (filename == null)
            {
                throw new ArgumentNullException(nameof(filename));
            }

            string extension = Path.GetExtension(filename).ToUpperInvariant();

            try
            {
                switch (extension)
                {
                    case ".TNT":
                        this.Save(filename);
                        return true;
                    case ".HPI":
                    case ".UFO":
                    case ".CCX":
                    case ".GPF":
                    case ".GP3":
                        this.SaveHpi(filename);
                        return true;
                    default:
                        this.dialogService.ShowError("Unrecognized file extension: " + extension);
                        return false;
                }
            }
            catch (IOException e)
            {
                this.dialogService.ShowError("Error saving map: " + e.Message);
                return false;
            }
        }

        private void SaveHpi(string filename)
        {
            // flatten before save --- only the base tile is written to disk
            IReplayableOperation flatten = OperationFactory.CreateFlattenOperation(this.model);
            flatten.Execute();

            this.mapSaver.SaveHpi(this.model, filename);

            flatten.Undo();

            this.undoManager.SetNowAsMark();

            this.FilePath = filename;
            this.IsFileReadOnly = false;
        }

        private void Save(string filename)
        {
            // flatten before save --- only the base tile is written to disk
            IReplayableOperation flatten = OperationFactory.CreateFlattenOperation(this.model);
            flatten.Execute();

            var otaName = filename.Substring(0, filename.Length - 4) + ".ota";
            this.mapSaver.SaveTnt(this.model, filename);
            this.mapSaver.SaveOta(this.model.Attributes, otaName);

            flatten.Undo();

            this.undoManager.SetNowAsMark();

            this.FilePath = filename;
            this.IsFileReadOnly = false;
        }

        private void PasteFeature(FeatureClipboardRecord feature, int x, int y)
        {
            this.DragDropFeature(feature.FeatureName, x, y);
        }

        private void PasteMapTile(IMapTile tile, int x, int y)
        {
            DeduplicateTiles(tile.TileGrid);
            this.PasteMapTileNoDeduplicate(tile, x, y);
        }

        private void PasteMapTileNoDeduplicate(IMapTile tile, int x, int y)
        {
            int normX = x / 32;
            int normY = y / 32;

            normX -= tile.TileGrid.Width / 2;
            normY -= tile.TileGrid.Height / 2;

            this.AddAndSelectTile(tile, normX, normY);
        }

        private bool TryCopyToClipboard()
        {
            if (this.SelectedFeatures.Count > 0)
            {
                var id = this.SelectedFeatures.First();
                var inst = this.GetFeatureInstance(id);
                var rec = new FeatureClipboardRecord(inst.FeatureName);
                Clipboard.SetData(DataFormats.Serializable, rec);
                return true;
            }

            if (this.SelectedTile.HasValue)
            {
                var tile = this.FloatingTiles[this.SelectedTile.Value].Item;
                Clipboard.SetData(DataFormats.Serializable, tile);
                return true;
            }

            return false;
        }

        private void FloatingTilesOnListChanged(object sender, ListChangedEventArgs e)
        {
            this.TilesChanged?.Invoke(this, e);
        }

        private void TileOnTileGridChanged(object sender, GridEventArgs e)
        {
            this.BaseTileGraphicsChanged?.Invoke(this, e);
        }

        private void TileOnHeightGridChanged(object sender, GridEventArgs e)
        {
            this.BaseTileHeightChanged?.Invoke(this, e);
        }

        private void AttributesOnStartPositionChanged(object sender, StartPositionChangedEventArgs e)
        {
            this.StartPositionChanged?.Invoke(this, e);
        }

        private void AddAndSelectTile(IMapTile tile, int x, int y)
        {
            var floatingSection = new Positioned<IMapTile>(tile, new Point(x, y));
            var addOp = new AddFloatingTileOperation(this.model, floatingSection);

            // Tile's index should always be 0,
            // because all other tiles are merged before adding this one.
            var index = 0;

            var selectOp = new SelectTileOperation(this.model, index);
            var op = new CompositeOperation(
                OperationFactory.CreateDeselectAndMergeOperation(this.model),
                addOp,
                selectOp);

            this.undoManager.Execute(op);
        }

        private Point? ScreenToHeightIndex(int x, int y)
        {
            return Util.ScreenToHeightIndex(this.model.Tile.HeightGrid, new Point(x, y));
        }

        private bool TranslateFeatureBatch(ICollection<Guid> ids, int x, int y)
        {
            if (x == 0 && y == 0)
            {
                return true;
            }

            var coordSet = new HashSet<GridCoordinates>(ids.Select(i => this.GetFeatureInstance(i).Location));

            // pre-move check to see if anything is in our way
            foreach (var item in coordSet)
            {
                var translatedPoint = new GridCoordinates(item.X + x, item.Y + y);

                if (translatedPoint.X < 0
                    || translatedPoint.Y < 0
                    || translatedPoint.X >= this.FeatureGridWidth
                    || translatedPoint.Y >= this.FeatureGridHeight)
                {
                    return false;
                }

                bool isBlocked = !coordSet.Contains(translatedPoint)
                    && this.HasFeatureInstanceAt(translatedPoint.X, translatedPoint.Y);
                if (isBlocked)
                {
                    return false;
                }
            }

            var newOp = new BatchMoveFeatureOperation(this.model, ids, x, y);

            BatchMoveFeatureOperation lastOp = null;
            if (this.undoManager.CanUndo)
            {
                lastOp = this.undoManager.PeekUndo() as BatchMoveFeatureOperation;
            }

            if (this.previousTranslationOpen && lastOp != null && lastOp.CanCombine(newOp))
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

        private void TranslateSection(int index, int x, int y)
        {
            this.TranslateSection(this.FloatingTiles[index], x, y);
        }

        private void TranslateSection(Positioned<IMapTile> tile, int x, int y)
        {
            if (tile.Location.X + tile.Item.TileGrid.Width + x <= 0)
            {
                x = -tile.Location.X - (tile.Item.TileGrid.Width - 1);
            }

            if (tile.Location.Y + tile.Item.TileGrid.Height + y <= 0)
            {
                y = -tile.Location.Y - (tile.Item.TileGrid.Height - 1);
            }

            if (tile.Location.X + x >= this.MapWidth)
            {
                x = this.MapWidth - tile.Location.X - 1;
            }

            if (tile.Location.Y + x >= this.MapHeight)
            {
                y = this.MapHeight - tile.Location.Y - 1;
            }

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

        private void TranslateStartPosition(int i, int x, int y)
        {
            var startPos = this.model.Attributes.GetStartPosition(i);

            if (startPos == null)
            {
                throw new ArgumentException("Start position " + i + " has not been placed");
            }

            this.TranslateStartPositionTo(i, startPos.Value.X + x, startPos.Value.Y + y);
        }

        private void TranslateStartPositionTo(int i, int x, int y)
        {
            var newOp = new ChangeStartPositionOperation(this.model, i, new Point(x, y));

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

        private void BandboxBehaviourPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "BandboxRectangle":
                    this.FireChange("BandboxRectangle");
                    break;
            }
        }

        private void SelectedFeaturesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateCanCut();
            this.UpdateCanCopy();
        }

        private void UpdateCanCopy()
        {
            this.CanCopy = this.SelectedTile.HasValue || this.SelectedFeatures.Count > 0;
        }

        private void UpdateCanCut()
        {
            this.CanCut = this.SelectedTile.HasValue || this.SelectedFeatures.Count > 0;
        }

        private void UndoManagerOnIsMarkedChanged(object sender, EventArgs eventArgs)
        {
            this.FireChange("IsMarked");
        }

        private void UndoManagerOnCanRedoChanged(object sender, EventArgs eventArgs)
        {
            this.FireChange("CanRedo");
        }

        private void UndoManagerOnCanUndoChanged(object sender, EventArgs eventArgs)
        {
            this.FireChange("CanUndo");
        }

        private MapAttributesResult GetAttributes()
        {
            return MapAttributesResult.FromModel(this.model);
        }

        private void UpdateAttributes(MapAttributesResult newAttrs)
        {
            this.undoManager.Execute(new ChangeAttributesOperation(this.model, newAttrs));
        }

        private void ModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            this.FireChange(propertyChangedEventArgs.PropertyName);

            switch (propertyChangedEventArgs.PropertyName)
            {
                case "SelectedTile":
                    this.UpdateCanCut();
                    this.UpdateCanCopy();
                    break;
            }
        }
    }
}
