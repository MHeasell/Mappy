namespace Mappy.Operations
{
    using System.Drawing;

    using Mappy.Models;

    public class UpdateMinimapOperation : IReplayableOperation
    {
        private readonly IMapModel model;

        private readonly Bitmap minimapImage;

        private Bitmap previousImage;

        public UpdateMinimapOperation(IMapModel model, Bitmap minimapImage)
        {
            this.model = model;
            this.minimapImage = minimapImage;
        }

        public void Execute()
        {
            this.previousImage = this.model.Minimap;
            this.model.Minimap = this.minimapImage;
        }

        public void Undo()
        {
            this.model.Minimap = this.previousImage;
        }
    }
}
