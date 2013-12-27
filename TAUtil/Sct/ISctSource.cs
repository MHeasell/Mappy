namespace TAUtil.Sct
{
    using System.Collections.Generic;

    using TAUtil.Tnt;

    public interface ISctSource
    {
        int DataWidth { get; }

        int DataHeight { get; }

        int TileCount { get; }

        IEnumerable<byte[]> EnumerateTiles();

        IEnumerable<int> EnumerateData();

        IEnumerable<TileAttr> EnumerateAttrs();

        byte[] GetMinimap();
    }
}