namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;

    public enum SectionType
    {
        Section,
        Feature
    }

    public interface ISectionViewViewModel
    {
        SectionType SectionType { get; set; }

        IObservable<ComboBoxViewModel> ComboBox1Model { get; }

        IObservable<ComboBoxViewModel> ComboBox2Model { get; }

        IObservable<IEnumerable<ListViewItem>> ListViewItems { get; }

        void SelectComboBox1Item(int index);

        void SelectComboBox2Item(int index);
    }
}