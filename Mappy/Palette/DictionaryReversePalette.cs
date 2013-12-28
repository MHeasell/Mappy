namespace Mappy.Palette
{
    using System.Collections.Generic;
    using System.Drawing;

    public class DictionaryReversePalette : IReversePalette
    {
        private readonly IDictionary<Color, int> lookup;

        public DictionaryReversePalette(IDictionary<Color, int> lookup)
        {
            this.lookup = lookup;
        }

        public int this[Color color]
        {
            get { return this.lookup[color]; }
        }
    }
}
