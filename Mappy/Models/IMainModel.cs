namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;

    using Mappy.Collections;
    using Mappy.Data;
    using Mappy.Database;

    public interface IMainModel : INotifyPropertyChanged
    {
        event EventHandler<SparseGridEventArgs> FeaturesChanged;

        event EventHandler<ListChangedEventArgs> TilesChanged;

        event EventHandler<GridEventArgs> BaseTileGraphicsChanged;

        event EventHandler<GridEventArgs> BaseTileHeightChanged;

        event EventHandler<StartPositionChangedEventArgs> StartPositionChanged;

        IFeatureDatabase FeatureRecords { get; }

        IList<Section> Sections { get; }

        bool CanUndo { get; }

        bool CanRedo { get; }

        bool CanCut { get; }

        bool CanCopy { get; }

        bool CanPaste { get; }

        bool IsDirty { get; }

        bool MapOpen { get; }

        string FilePath { get; }

        bool IsFileOpen { get; }

        bool IsFileReadOnly { get; }

        bool GridVisible { get; set; }

        Color GridColor { get; set; }

        Size GridSize { get; set; }

        bool HeightmapVisible { get; set; }

        bool FeaturesVisible { get; set; }

        int? SelectedTile { get; }

        int? SelectedStartPosition { get; }

        Rectangle BandboxRectangle { get; }

        ICollection<GridCoordinates> SelectedFeatures { get; }

        ISparseGrid<Feature> Features { get; }

        IList<Positioned<IMapTile>> FloatingTiles { get; }

        IMapTile BaseTile { get; }

        int MapWidth { get; }

        int MapHeight { get; }

        int SeaLevel { get; set; }

        void DragDropStartPosition(int index, int x, int y);

        void DragDropTile(int id, int x, int y);

        void DragDropFeature(string name, int x, int y);

        void StartBandbox(int x, int y);

        void GrowBandbox(int x, int y);

        void CommitBandbox();

        void TranslateSelection(int x, int y);

        void FlushTranslation();

        void ClearSelection();

        void DeleteSelection();

        Point? GetStartPosition(int index);

        void SelectTile(int index);

        void SelectFeature(GridCoordinates index);

        void SelectStartPosition(int index);

        void Undo();

        void Redo();

        void New(int width, int height);

        void Save(string filename);

        void SaveHpi(string filename);

        void OpenTnt(string filename);

        void OpenSct(string filename);

        void OpenHapi(string archivePath, string filename, bool readOnly = false);

        void RefreshMinimap();

        void RefreshMinimapHighQuality();

        MapAttributesResult GetAttributes();

        void UpdateAttributes(MapAttributesResult attributes);

        void CloseMap();

        void SetSeaLevel(int value);

        void FlushSeaLevel();

        void CutSelectionToClipboard();

        void CopySelectionToClipboard();

        void PasteFromClipboard();
    }
}