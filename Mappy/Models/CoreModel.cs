namespace Mappy.Models
{
    using System.ComponentModel;
    using System.Drawing;

    using Mappy.Util;

    public class CoreModel : Notifier, IReadOnlyApplicationModel
    {
        private UndoableMapModel map;

        private bool heightmapVisible;
        private bool featuresVisible = true;

        private bool minimapVisible;

        private bool gridVisible;
        private Size gridSize = new Size(16, 16);
        private Color gridColor = MappySettings.Settings.GridColor;

        private int viewportWidth;
        private int viewportHeight;

        public UndoableMapModel Map
        {
            get
            {
                return this.map;
            }

            set
            {
                if (this.SetField(ref this.map, value, "Map"))
                {
                    if (this.Map != null)
                    {
                        this.Map.PropertyChanged += this.MapOnPropertyChanged;
                    }

                    this.FireChange("CanUndo");
                    this.FireChange("CanRedo");

                    this.FireChange("CanCut");
                    this.FireChange("CanCopy");
                    this.FireChange("CanPaste");
                }
            }
        }

        public bool CanUndo => this.Map != null && this.Map.CanUndo;

        public bool CanRedo => this.Map != null && this.Map.CanRedo;

        public bool CanCopy => this.Map != null && this.Map.CanCopy;

        public bool CanPaste => this.Map != null;

        public bool CanCut => this.Map != null && this.Map.CanCut;

        public bool HeightmapVisible
        {
            get { return this.heightmapVisible; }
            set { this.SetField(ref this.heightmapVisible, value, "HeightmapVisible"); }
        }

        public bool FeaturesVisible
        {
            get { return this.featuresVisible; }
            set { this.SetField(ref this.featuresVisible, value, "FeaturesVisible"); }
        }

        public int ViewportWidth
        {
            get
            {
                return this.viewportWidth;
            }

            set
            {
                this.SetField(ref this.viewportWidth, value, "ViewportWidth");
            }
        }

        public int ViewportHeight
        {
            get
            {
                return this.viewportHeight;
            }

            set
            {
                this.SetField(ref this.viewportHeight, value, "ViewportHeight");
            }
        }

        public bool MinimapVisible
        {
            get
            {
                return this.minimapVisible;
            }

            set
            {
                this.SetField(ref this.minimapVisible, value, "MinimapVisible");
            }
        }

        public bool GridVisible
        {
            get { return this.gridVisible; }
            set { this.SetField(ref this.gridVisible, value, "GridVisible"); }
        }

        public Size GridSize
        {
            get { return this.gridSize; }
            set { this.SetField(ref this.gridSize, value, "GridSize"); }
        }

        public Color GridColor
        {
            get
            {
                return this.gridColor;
            }

            set
            {
                MappySettings.Settings.GridColor = value;
                MappySettings.SaveSettings();
                this.SetField(ref this.gridColor, value, "GridColor");
            }
        }

        public void SetViewportLocation(Point location)
        {
            if (this.Map == null)
            {
                return;
            }

            location.X = Util.Clamp(location.X, 0, (this.Map.MapWidth * 32) - this.ViewportWidth);
            location.Y = Util.Clamp(location.Y, 0, (this.Map.MapHeight * 32) - this.ViewportHeight);

            this.Map.ViewportLocation = location;
        }

        public void SetViewportSize(Size size)
        {
            this.ViewportWidth = size.Width;
            this.ViewportHeight = size.Height;
        }

        private void MapOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            switch (propertyChangedEventArgs.PropertyName)
            {
                case "CanUndo":
                case "CanRedo":
                case "CanCut":
                case "CanCopy":
                case "CanPaste":
                    this.FireChange(propertyChangedEventArgs.PropertyName);
                    break;
            }
        }
    }
}
