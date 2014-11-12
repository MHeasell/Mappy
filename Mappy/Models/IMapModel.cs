namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using Data;

    using Mappy.Collections;

    public interface IMapModel
    {
        IMapTile Tile { get; }

        IList<Positioned<IMapTile>> FloatingTiles { get; }

        ISparseGrid<bool> Voids { get; }

        Bitmap Minimap { get; set; }

        int SeaLevel { get; set; }

        MapAttributes Attributes { get; }

        /// <summary>
        /// Gets the width of the feature grid space.
        /// </summary>
        int FeatureGridWidth { get; }

        /// <summary>
        /// Gets the height of the feature grid space.
        /// </summary>
        int FeatureGridHeight { get; }

        /// <summary>
        /// Adds the given feature instance.
        /// The location must be within map boundaries or an exception is raised.
        /// If there is another feature is at the given location,
        /// an acception is raised.
        /// </summary>
        /// <param name="instance">The instance to add.</param>
        void AddFeatureInstance(FeatureInstance instance);

        /// <summary>
        /// Updates the given instance in the collection,
        /// replacing the existing instance with the same ID.
        /// If the ID is not present already, an exception is raised.
        /// </summary>
        /// <param name="instance">The instance to update.</param>
        void UpdateFeatureInstance(FeatureInstance instance);

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

        /// <summary>
        /// Removes the feature instance with the given ID.
        /// </summary>
        /// <param name="id">The ID of the instance to remove.</param>
        void RemoveFeatureInstance(Guid id);

        /// <returns>
        /// An enumeration of all FeatureInstance objects
        /// contained in this map.
        /// </returns>
        IEnumerable<FeatureInstance> EnumerateFeatureInstances();
    }
}
