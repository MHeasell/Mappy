namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using Mappy.Data;
    using Mappy.Database;

    public interface IMainFormModel
    {
        IObservable<IFeatureDatabase> FeatureRecords { get; }

        IObservable<IList<Section>> Sections { get; }

        IObservable<bool> CanUndo { get; }

        IObservable<bool> CanRedo { get; }

        IObservable<bool> CanCut { get; }

        IObservable<bool> CanCopy { get; }

        IObservable<bool> CanPaste { get; }

        IObservable<bool> IsDirty { get; }

        IObservable<bool> MapOpen { get; }

        IObservable<string> FilePath { get; }

        IObservable<bool> IsFileReadOnly { get; }

        IObservable<bool> GridVisible { get; }

        IObservable<Size> GridSize { get; }

        IObservable<bool> HeightmapVisible { get; }

        IObservable<bool> FeaturesVisible { get; }

        IObservable<bool> MinimapVisible { get; }

        IObservable<int> SeaLevel { get; }
    }
}
