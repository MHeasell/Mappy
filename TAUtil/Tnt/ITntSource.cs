namespace TAUtil.Tnt
{
    using System.Collections.Generic;

    public interface ITntSource
    {
        int DataWidth { get; }

        int DataHeight { get; }

        int SeaLevel { get; }

        int TileCount { get; }

        int AnimCount { get; }

        IEnumerable<byte[]> EnumerateTiles();

        IEnumerable<string> EnumerateAnims();

        IEnumerable<int> EnumerateData();

        IEnumerable<TileAttr> EnumerateAttrs();

        MinimapInfo GetMinimap();
    }
}