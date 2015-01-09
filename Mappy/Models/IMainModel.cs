namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;

    using Geometry;

    using Mappy.Collections;
    using Mappy.Data;
    using Mappy.Database;

    public interface IMainModel : INotifyPropertyChanged
    {
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

        bool MinimapVisible { get; set; }

        int? SelectedTile { get; }

        int? SelectedStartPosition { get; }

        Rectangle BandboxRectangle { get; }

        ISelectionModel Map { get; }

        IList<Positioned<IMapTile>> FloatingTiles { get; }

        IMapTile BaseTile { get; }

        int MapWidth { get; }

        int MapHeight { get; }

        int SeaLevel { get; set; }

        Rectangle2D ViewportRectangle { get; set; }

        void Initialize();

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

        void SelectFeature(Guid id);

        void SelectStartPosition(int index);

        void SetGridSize(int size);

        void ChooseColor();

        void OpenMapAttributes();

        void Undo();

        void Redo();

        bool New();

        bool Save();

        bool SaveAs();

        bool Open();

        void Close();

        void ShowAbout();

        void ToggleHeightmap();

        void ToggleMinimap();

        void ToggleFeatures();

        void RefreshMinimap();

        void RefreshMinimapHighQuality();

        void RefreshMinimapHighQualityWithProgress();

        void OpenPreferences();

        void CloseMap();

        void SetSeaLevel(int value);

        void FlushSeaLevel();

        void CutSelectionToClipboard();

        void CopySelectionToClipboard();

        void PasteFromClipboard();

        void ExportMinimap();

        void ExportHeightmap();

        void ImportMinimap();

        void ImportHeightmap();

        void ImportCustomSection();

        void ExportMapImage();
    }
}