namespace Mappy.Models
{
    using System.ComponentModel;
    using System.Drawing;

    using Mappy.Util;

    public class CoreModel : Notifier, IReadOnlyApplicationModel
    {
        private Maybe<UndoableMapModel> map;

        private bool heightmapVisible;
        private bool voidsVisible;
        private bool featuresVisible = true;

        private bool minimapVisible;

        private bool gridVisible;
        private Size gridSize = new Size(16, 16);
        private Color gridColor = MappySettings.Settings.GridColor;

        private int viewportWidth;
        private int viewportHeight;

        public Maybe<UndoableMapModel> Map
        {
            get => this.map;

            set
            {
                if (this.SetField(ref this.map, value, nameof(this.Map)))
                {
                    this.Map.IfSome(x => x.PropertyChanged += this.MapOnPropertyChanged);

                    this.OnPropertyChanged("CanUndo");
                    this.OnPropertyChanged("CanRedo");

                    this.OnPropertyChanged("CanCut");
                    this.OnPropertyChanged("CanCopy");
                    this.OnPropertyChanged("CanPaste");
                }
            }
        }

        public bool CanUndo => this.Map.Match(x => x.CanUndo, () => false);

        public bool CanRedo => this.Map.Match(x => x.CanRedo, () => false);

        public bool CanCopy => this.Map.Match(x => x.CanCopy, () => false);

        public bool CanPaste => this.Map.IsSome;

        public bool CanCut => this.Map.Match(x => x.CanCut, () => false);

        public bool HeightmapVisible
        {
            get => this.heightmapVisible;
            set => this.SetField(ref this.heightmapVisible, value, nameof(this.HeightmapVisible));
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

        private void MapOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            switch (propertyChangedEventArgs.PropertyName)
            {
                case "CanUndo":
                case "CanRedo":
                case "CanCut":
                case "CanCopy":
                case "CanPaste":
                    this.OnPropertyChanged(propertyChangedEventArgs.PropertyName);
                    break;
            }
        }
    }
}
