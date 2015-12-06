namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;

    public interface ISectionViewViewModel
    {
        IObservable<ComboBoxViewModel> Worlds { get; }

        IObservable<ComboBoxViewModel> Categories { get; }

        IObservable<IEnumerable<ListViewItem>> Sections { get; }

        void SelectWorld(int index);

        void SelectCategory(int index);
    }
}