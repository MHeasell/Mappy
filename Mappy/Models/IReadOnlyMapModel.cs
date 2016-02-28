namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using Mappy.Collections;
    using Mappy.Data;

    public interface IReadOnlyMapModel
    {
        IMapTile Tile { get; }

        IList<Positioned<IMapTile>> FloatingTiles { get; }

        ISparseGrid<bool> Voids { get; }

        Bitmap Minimap { get; }

        int SeaLevel { get; }

        MapAttributes Attributes { get; }

        /// <summary>
        /// Gets the width of the feature grid space.
        /// </summary>
        int FeatureGridWidth { get; }

        /// <summary>
        /// Gets the height of the feature grid space.
        /// </summary>
        int FeatureGridHeight { get; }

        /// <param name="id">The ID of the instance to retrieve.</param>
        /// <returns>
        /// The feature instance with the given ID,
        /// or null if it is not found.
        /// </returns>
        FeatureInstance GetFeatureInstance(Guid id);

        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns>
        /// True if the model contains a feature instance
        /// at the specified location.
        /// </returns>
        bool HasFeatureInstanceAt(int x, int y);

        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns>
        /// The feature instance at the given location,
        /// or null if there is no feature instance at the location.
        /// </returns>
        FeatureInstance GetFeatureInstanceAt(int x, int y);

        /// <returns>
        /// An enumeration of all FeatureInstance objects
        /// contained in this map.
        /// </returns>
        IEnumerable<FeatureInstance> EnumerateFeatureInstances();
    }
}