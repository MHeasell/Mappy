namespace Mappy.Models
{
    using System;
    using System.Drawing;

    public interface IMapModel : IReadOnlyMapModel
    {
        new Bitmap Minimap { get; set; }

        new int SeaLevel { get; set; }

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

        /// <summary>
        /// Removes the feature instance with the given ID.
        /// </summary>
        /// <param name="id">The ID of the instance to remove.</param>
        void RemoveFeatureInstance(Guid id);
    }
}
