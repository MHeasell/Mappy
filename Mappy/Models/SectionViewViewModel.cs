namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Reactive.Subjects;

    using Mappy.Data;

    public struct ComboBoxViewModel
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

    public struct ListViewItem
    {
        public ListViewItem(string name, Bitmap image, object tag)
        {
            this.Name = name;
            this.Image = image;
            this.Tag = tag;
        }

        public string Name { get; }

        public Bitmap Image { get; }

        public object Tag { get; }
    }

    public class SectionViewViewModel : ISectionViewViewModel
    {
        private readonly BehaviorSubject<ComboBoxViewModel> worlds = new BehaviorSubject<ComboBoxViewModel>(ComboBoxViewModel.Empty);

        private readonly BehaviorSubject<ComboBoxViewModel> categories = new BehaviorSubject<ComboBoxViewModel>(ComboBoxViewModel.Empty);

        private readonly Subject<int> selectWorldEvent = new Subject<int>();

        private readonly Subject<int> selectCategoryEvent = new Subject<int>();

        private readonly BehaviorSubject<IList<Section>> rawSections;

        private readonly BehaviorSubject<IEnumerable<ListViewItem>> sections = new BehaviorSubject<IEnumerable<ListViewItem>>(Enumerable.Empty<ListViewItem>());

        public SectionViewViewModel(CoreModel model)
        {
            this.rawSections = model.PropertyAsObservable(x => x.Sections, "Sections");

            this.rawSections.Subscribe(
                sections =>
                    {
                        this.UpdateWorlds();
                        this.UpdateCategories();
                        this.UpdateSections();
                    });

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

        private static IEnumerable<string> EnumerateWorlds(IEnumerable<Section> sections)
        {
            return sections
                .Select(x => x.World)
                .Distinct(StringComparer.InvariantCultureIgnoreCase)
                .OrderBy(x => x, StringComparer.InvariantCultureIgnoreCase);
        }

        private static IEnumerable<string> EnumerateCategories(IEnumerable<Section> sections, string world)
        {
            return sections
                .Where(x => x.World == world)
                .Select(x => x.Category)
                .Distinct(StringComparer.InvariantCultureIgnoreCase)
                .OrderBy(x => x, StringComparer.InvariantCultureIgnoreCase);
        }

        private static IEnumerable<Section> FilterSections(IEnumerable<Section> sections, string world, string category)
        {
            return sections
                .Where(x => x.World == world && x.Category == category)
                .OrderBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase);
        }

        private static ListViewItem ToItem(Section s)
        {
            var label = $"{s.Name} ({s.PixelWidth}x{s.PixelHeight})";
            return new ListViewItem(label, s.Minimap, s.Id.ToString());
        }

        private void UpdateWorlds()
        {
            var sections = this.rawSections.Value;

            var worlds = EnumerateWorlds(sections).ToList();
            var worldsModel = new ComboBoxViewModel(worlds);
            this.worlds.OnNext(worldsModel);
        }

        private void UpdateCategories()
        {
            var sections = this.rawSections.Value;
            var worldsModel = this.worlds.Value;

            var categories = EnumerateCategories(sections, worldsModel.SelectedItem).ToList();
            var categoriesModel = new ComboBoxViewModel(categories);
            this.categories.OnNext(categoriesModel);
        }

        private void UpdateSections()
        {
            var filteredSections = FilterSections(
                this.rawSections.Value,
                this.worlds.Value.SelectedItem,
                this.categories.Value.SelectedItem);
            this.sections.OnNext(filteredSections.Select(ToItem));
        }
    }
}
