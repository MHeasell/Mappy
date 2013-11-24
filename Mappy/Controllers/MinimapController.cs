namespace Mappy.Controllers
{
    using System.Drawing;
    using Views;

    public class MinimapController
    {
        private IMinimapView minimap;
        private IMainView mainView;

        public MinimapController(IMinimapView mini, IMainView main)
        {
            this.minimap = mini;
            this.mainView = main;

            this.minimap.ViewportMove += this.ViewportMove;
        }

        private void ViewportMove(object sender, MinimapMoveEventArgs e)
        {
            this.UpdateMinimapFromPoint(e.Location);
        }

        private void UpdateMinimapFromPoint(Point p)
        {
            if (this.mainView.Map == null)
            {
                return;
            }

            float facX = this.mainView.Map.Minimap.Width / (this.mainView.Map.Tile.TileGrid.Width * 32.0f);
            float facY = this.mainView.Map.Minimap.Height / (this.mainView.Map.Tile.TileGrid.Height * 32.0f);
            p.X = (int)(p.X / facX);
            p.Y = (int)(p.Y / facY);
            this.mainView.SetViewportCenter(p);
        }
    }
}
