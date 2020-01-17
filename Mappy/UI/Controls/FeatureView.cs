namespace Mappy.UI.Controls {
    using System.Windows.Forms;
    using Mappy.Models;

    public partial class FeatureView : SectionView
    {
        public FeatureView()
            : base()
        {
            this.InitializeComponent();
            this.ActiveFeaturePlacementMode = FeaturePlacementMode.Selection;
        }

        public FeaturePlacementMode ActiveFeaturePlacementMode { get; set; }

        private void LineBtn_MouseClick(object sender, MouseEventArgs e) {
            this.ActiveFeaturePlacementMode = FeaturePlacementMode.Line;
        }

        private void SelectionBtn_MouseClick(object sender, MouseEventArgs e) {
            this.ActiveFeaturePlacementMode = FeaturePlacementMode.Selection;
        }

        private void FillBtn_MouseClick(object sender, MouseEventArgs e) {
            this.ActiveFeaturePlacementMode = FeaturePlacementMode.Fill;
        }

        private void SporadicBtn_MouseClick(object sender, MouseEventArgs e) {
            this.ActiveFeaturePlacementMode = FeaturePlacementMode.Sporadic;
        }
    }
}
