namespace Mappy.UI.Controls
{
    public class StartPositionDragData
    {
        public StartPositionDragData(int i)
        {
            this.PositionNumber = i;
        }

        public int PositionNumber { get; private set; }
    }
}