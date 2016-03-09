namespace Mappy.UI.Forms
{
    using System;
    using System.Drawing;
    using System.Reactive.Linq;
    using System.Windows.Forms;

    using Mappy.Models;

    public partial class MinimapForm : Form
    {
        private static readonly Color[] StartPositionColors = new[]
            {
                Color.FromArgb(0, 0, 255),
                Color.FromArgb(255, 0, 0),
                Color.FromArgb(255, 255, 255),
                Color.FromArgb(0, 255, 0),
                Color.FromArgb(0, 0, 128),
                Color.FromArgb(128, 0, 255),
                Color.FromArgb(255, 255, 0),
                Color.FromArgb(0, 0, 0),
                Color.FromArgb(128, 128, 255),
                Color.FromArgb(255, 180, 140),
            };

        private IMinimapFormViewModel model;

        public MinimapForm()
        {
            this.InitializeComponent();
        }

        public void SetModel(IMinimapFormViewModel model)
        {
            model.PropertyAsObservable(x => x.MinimapVisible, nameof(model.MinimapVisible))
                .Subscribe(x => this.Visible = x);

            model.PropertyAsObservable(x => x.MinimapImage, nameof(model.MinimapImage))
                .Select(x => x.Or(null))
                .Subscribe(x => this.minimapControl.BackgroundImage = x);

            model.PropertyAsObservable(x => x.MinimapRect, nameof(model.MinimapRect))
                .Subscribe(x => this.minimapControl.ViewportRect = x);

            for (var i = 0; i < model.StartPositions.Count; i++)
            {
                var i1 = i;
                var pos = model.StartPositions[i];
                pos.Subscribe(x => x.Do(
                            y => this.minimapControl.SetMarker(i1, y, StartPositionColors[i1]),
                            () => this.minimapControl.RemoveMarker(i1)));
            }

            this.model = model;
        }

        private void MinimapFormFormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;

                this.model.FormCloseButtonClick();
            }
        }

        private void MinimapControl1MouseDown(object sender, MouseEventArgs e)
        {
            this.model.MouseDown(e.Location);
        }

        private void MinimapControl1MouseMove(object sender, MouseEventArgs e)
        {
            this.model.MouseMove(e.Location);
        }

        private void MinimapControl1MouseUp(object sender, MouseEventArgs e)
        {
            this.model.MouseUp();
        }
    }
}
