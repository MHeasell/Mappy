namespace Mappy.Models
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public class ComboBoxViewModel
    {
        [SuppressMessage("Microsoft.Security", "CA2104", Justification = "This type is intended to be immutable")]
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

        // IList is technically a mutable type.
        // TODO: Replace with IReadOnlyList or an immutable list
        //       after upgrading to a newer version of .NET.
        public IList<string> Items { get; }

        public int SelectedIndex { get; }

        public string SelectedItem => this.SelectedIndex == -1 ? null : this.Items[this.SelectedIndex];

        public ComboBoxViewModel Select(int index) => new ComboBoxViewModel(this.Items, index);
    }
}