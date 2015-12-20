namespace Mappy.UI.Controls
{
    using System.Collections.Generic;
    using System.Drawing;

    public class GuideLayer : AbstractLayer
    {
        private static readonly Pen Pen = new Pen(Color.Magenta, 3.0f);

        private readonly List<int> horizontalGuides = new List<int>();
        private readonly List<int> verticalGuides = new List<int>();

        public void AddHorizontalGuide(int yPosition)
        {
            this.horizontalGuides.Add(yPosition);
            this.InvalidateHorizontalGuide(yPosition);
        }

        public void AddVerticalGuide(int xPosition)
        {
            this.verticalGuides.Add(xPosition);
            this.InvalidateVerticalGuide(xPosition);
        }

        public void ClearGuides()
        {
            var hCopy = new List<int>(this.horizontalGuides);
            var vCopy = new List<int>(this.verticalGuides);

            this.verticalGuides.Clear();
            this.horizontalGuides.Clear();

            foreach (var y in hCopy)
            {
                this.InvalidateHorizontalGuide(y);
            }

            foreach (var x in vCopy)
            {
                this.InvalidateVerticalGuide(x);
            }
        }

        protected override void DoDraw(Graphics graphics, Rectangle clipRectangle)
        {
            foreach (var y in this.horizontalGuides)
            {
                graphics.DrawLine(Pen, clipRectangle.Left, y, clipRectangle.Right, y);
            }

            foreach (var x in this.verticalGuides)
            {
                graphics.DrawLine(Pen, x, clipRectangle.Top, x, clipRectangle.Bottom);
            }
        }

        private void InvalidateVerticalGuide(int xPosition)
        {
            this.OnLayerChanged(new Rectangle(xPosition, 0, 3, int.MaxValue));
        }

        private void InvalidateHorizontalGuide(int yPosition)
        {
            this.OnLayerChanged(new Rectangle(0, yPosition, int.MaxValue, 3));
        }
    }
}
