namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Subjects;

    using Mappy.Data;
    using Mappy.Services;

    public sealed class SectionViewViewModel : ISectionViewViewModel, IDisposable
    {
        private readonly BehaviorSubject<ComboBoxViewModel> worlds = new BehaviorSubject<ComboBoxViewModel>(ComboBoxViewModel.Empty);

        private readonly BehaviorSubject<ComboBoxViewModel> categories = new BehaviorSubject<ComboBoxViewModel>(ComboBoxViewModel.Empty);

        private readonly BehaviorSubject<IEnumerable<ListViewItem>> sections = new BehaviorSubject<IEnumerable<ListViewItem>>(Enumerable.Empty<ListViewItem>());

        private readonly Subject<int> selectWorldEvent = new Subject<int>();

        private readonly Subject<int> selectCategoryEvent = new Subject<int>();

        private readonly SectionService sectionService;

        private readonly Dispatcher dispatcher;

        public SectionViewViewModel(SectionService sectionService, Dispatcher dispatcher)
        {
            sectionService.SectionsChanged += this.OnSectionsChanged;

            this.selectWorldEvent.Subscribe(
                i =>
                    {
                        this.worlds.OnNext(this.worlds.Value.Select(i));

                        this.UpdateCategories();
                        this.UpdateSections();
                    });

            this.selectCategoryEvent.Subscribe(
                i =>
                    {
                        this.categories.OnNext(this.categories.Value.Select(i));

                        this.UpdateSections();
                    });

            this.sectionService = sectionService;
            this.dispatcher = dispatcher;
        }

        public IObservable<ComboBoxViewModel> ComboBox1Model => this.worlds;

        public IObservable<ComboBoxViewModel> ComboBox2Model => this.categories;

        public IObservable<IEnumerable<ListViewItem>> ListViewItems => this.sections;

        public void SelectComboBox1Item(int index)
        {
            this.selectWorldEvent.OnNext(index);
        }

        public void SelectComboBox2Item(int index)
        {
            this.selectCategoryEvent.OnNext(index);
        }

        public void SetSelectedFeature(string featureName)
        {
            this.dispatcher.SetSelectedFeature(featureName);
        }

        public void Dispose()
        {
            this.worlds.Dispose();
            this.categories.Dispose();
            this.sections.Dispose();
            this.selectWorldEvent.Dispose();
            this.selectCategoryEvent.Dispose();
        }

        private static ListViewItem ToItem(int id, Section s)
        {
            var label = $"{s.Name} ({s.PixelWidth}x{s.PixelHeight})";
            return new ListViewItem(label, s.Minimap, id.ToString());
        }

        private void UpdateWorlds()
        {
            var worlds = this.sectionService.EnumerateWorlds();
            var worldsModel = new ComboBoxViewModel(worlds.ToList());
            this.worlds.OnNext(worldsModel);
        }

        private void UpdateCategories()
        {
            var world = this.worlds.Value.SelectedItem;

            var categories = this.sectionService.EnumerateCategories(world);
            var categoriesModel = new ComboBoxViewModel(categories.ToList());
            this.categories.OnNext(categoriesModel);
        }

        private void UpdateSections()
        {
            var world = this.worlds.Value.SelectedItem;
            var category = this.categories.Value.SelectedItem;
            var sections = this.sectionService.EnumerateSections(world, category);
            this.sections.OnNext(sections.Select(x => ToItem(x.Key, x.Value)).ToList());
        }

        private void OnSectionsChanged(object sender, EventArgs e)
        {
            this.UpdateWorlds();
            this.UpdateCategories();
            this.UpdateSections();
        }
    }
}
