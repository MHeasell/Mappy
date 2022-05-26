namespace Mappy.Models
{
    using System.Drawing;
    using Mappy.Models.Enums;
    using Mappy.Util;

    public class CoreModel : Notifier, IReadOnlyApplicationModel
    {
        private Maybe<UndoableMapModel> map;

        private bool heightmapVisible;
        private bool heightGridVisible;
        private bool voidsVisible;
        private bool featuresVisible = true;

        private bool minimapVisible;

        private bool gridVisible;
        private Size gridSize = new Size(16, 16);
        private Color gridColor = MappySettings.Settings.GridColor;

        private GUITab guiTab = GUITab.Sections;

        private int viewportWidth;
        private int viewportHeight;

        public Maybe<UndoableMapModel> Map
        {
            get => this.map;
            set => this.SetField(ref this.map, value, nameof(this.Map));
        }

        public bool HeightmapVisible
        {
            get => this.heightmapVisible;
            set => this.SetField(ref this.heightmapVisible, value, nameof(this.HeightmapVisible));
        }

        public bool HeightGridVisible
        {
            get => this.heightGridVisible;
            set => this.SetField(ref this.heightGridVisible, value, nameof(this.HeightGridVisible));
        }

        public bool VoidsVisible
        {
            get => this.voidsVisible;
            set => this.SetField(ref this.voidsVisible, value, nameof(this.VoidsVisible));
        }

        public bool FeaturesVisible
        {
            get => this.featuresVisible;
            set => this.SetField(ref this.featuresVisible, value, nameof(this.FeaturesVisible));
        }

        public int ViewportWidth
        {
            get => this.viewportWidth;
            set => this.SetField(ref this.viewportWidth, value, nameof(this.ViewportWidth));
        }

        public int ViewportHeight
        {
            get => this.viewportHeight;
            set => this.SetField(ref this.viewportHeight, value, nameof(this.ViewportHeight));
        }

        public bool MinimapVisible
        {
            get => this.minimapVisible;
            set => this.SetField(ref this.minimapVisible, value, nameof(this.MinimapVisible));
        }

        public bool GridVisible
        {
            get => this.gridVisible;
            set => this.SetField(ref this.gridVisible, value, nameof(this.GridVisible));
        }

        public Size GridSize
        {
            get => this.gridSize;
            set => this.SetField(ref this.gridSize, value, nameof(this.GridSize));
        }

        public Color GridColor
        {
            get => this.gridColor;
            set
            {
                MappySettings.Settings.GridColor = value;
                MappySettings.SaveSettings();
                this.SetField(ref this.gridColor, value, nameof(this.GridColor));
            }
        }

        public GUITab SelectedGUITab
        {
            get => this.guiTab;
            set
            {
                this.SetField(ref this.guiTab, value, nameof(this.SelectedGUITab));
            }
        }

        public void SetViewportLocation(Point location)
        {
            this.Map.IfSome(
                map =>
                    {
                        location.X = Util.Clamp(location.X, 0, (map.MapWidth * 32) - this.ViewportWidth);
                        location.Y = Util.Clamp(location.Y, 0, (map.MapHeight * 32) - this.ViewportHeight);

                        map.ViewportLocation = location;
                    });
        }

        public void SetViewportSize(Size size)
        {
            this.ViewportWidth = size.Width;
            this.ViewportHeight = size.Height;
        }
    }
}
