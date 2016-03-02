namespace Mappy.Models
{
    using System.Collections.Generic;

    public class ComboBoxViewModel
    {
        public static readonly ComboBoxViewModel Empty = new ComboBoxViewModel(new List<string>(), -1);

        public ComboBoxViewModel(IList<string> items)
            : this(items, items.Count > 0 ? 0 : -1)
        {
        }

        public ComboBoxViewModel(IList<string> items, int selectedIndex)
        {
            this.Items = items;
            this.SelectedIndex = selectedIndex;
        }

        public IList<string> Items { get; }

        public int SelectedIndex { get; }

        public string SelectedItem => this.SelectedIndex == -1 ? null : this.Items[this.SelectedIndex];

        public ComboBoxViewModel Select(int index) => new ComboBoxViewModel(this.Items, index);
    }
}