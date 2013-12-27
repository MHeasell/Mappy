namespace TAUtil.Tnt
{
    public class MinimapInfo
    {
        public MinimapInfo(int width, int height, byte[] data)
        {
            this.Width = width;
            this.Height = height;
            this.Data = data;
        }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public byte[] Data { get; private set; }
    }
}