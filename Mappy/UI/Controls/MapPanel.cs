namespace Mappy.UI.Controls
{
    using System.Drawing;
    using System.Windows.Forms;

    public partial class MapPanel : UserControl
    {
        public MapPanel()
        {
            this.InitializeComponent();
        }

        public ImageLayerView MapControl
        {
            get
            {
                return this.mapControl;
            }
        }

        protected override Point ScrollToControl(Control activeControl)
        {
            // return current location ---
            // prevents the panel from scrolling back to the topleft
            // when it loses and regains focus
            return this.DisplayRectangle.Location;
        }
    }
}
