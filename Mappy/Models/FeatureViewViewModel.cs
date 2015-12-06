namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Reactive.Subjects;

    using Mappy.Data;
    using Mappy.Database;

    public class FeatureViewViewModel : ISectionViewViewModel
    {
        private readonly BehaviorSubject<ComboBoxViewModel> worlds = new BehaviorSubject<ComboBoxViewModel>(ComboBoxViewModel.Empty); 

        private readonly BehaviorSubject<ComboBoxViewModel> categories = new BehaviorSubject<ComboBoxViewModel>(ComboBoxViewModel.Empty);

        private readonly Subject<int> selectWorldEvent = new Subject<int>();

        private readonly Subject<int> selectCategoryEvent = new Subject<int>();

        private readonly BehaviorSubject<IFeatureDatabase> rawFeatures;

        private readonly BehaviorSubject<IEnumerable<ListViewItem>> features = new BehaviorSubject<IEnumerable<ListViewItem>>(Enumerable.Empty<ListViewItem>()); 

        private readonly Dictionary<string, Bitmap> rescaledImageMap = new Dictionary<string, Bitmap>();

        public FeatureViewViewModel(CoreModel model)
        {
            this.rawFeatures = model.PropertyAsObservable(x => x.FeatureRecords, "FeatureRecords");

            this.rawFeatures.Subscribe(
                _ =>
                    {
                        this.UpdateWorlds();
                        this.UpdateCategories();
                        this.UpdateFeatures();
                    });

            this.selectWorldEvent.Subscribe(
                i =>
                    {
                        this.worlds.OnNext(this.worlds.Value.Select(i));

                        this.UpdateCategories();
                        this.UpdateFeatures();
                    });

            this.selectCategoryEvent.Subscribe(
                i =>
                    {
                        this.categories.OnNext(this.categories.Value.Select(i));

                        this.UpdateFeatures();
                    });
        }

        public IObservable<ComboBoxViewModel> ComboBox1Model => this.worlds;

        public IObservable<ComboBoxViewModel> ComboBox2Model => this.categories;

        public IObservable<IEnumerable<ListViewItem>> ListViewItems => this.features;

        public void SelectComboBox1Item(int index)
        {
            this.selectWorldEvent.OnNext(index);
        }

        public void SelectComboBox2Item(int index)
        {
            this.selectCategoryEvent.OnNext(index);
        }

        private static Bitmap RescaleImage(Bitmap img)
        {
            int outWidth = 64;
            int outHeight = 64;

            Bitmap thumb = new Bitmap(outWidth, outHeight);
            Graphics g = Graphics.FromImage(thumb);

            double ratioX = outWidth / (double)img.Width;
            double ratioY = outHeight / (double)img.Height;

            // use the smaller ratio
            double ratio = Math.Min(ratioX, ratioY);

            int newWidth;
            int newHeight;
            if (img.Width <= outWidth && img.Height <= outHeight)
            {
                // keep original image size if smaller
                newWidth = img.Width;
                newHeight = img.Height;
            }
            else
            {
                newWidth = (int)(img.Width * ratio);
                newHeight = (int)(img.Height * ratio);
            }

            int posX = (outWidth - newWidth) / 2;
            int posY = (outHeight - newHeight) / 2;

            g.DrawImage(img, posX, posY, newWidth, newHeight);

            return thumb;
        }

        private static IEnumerable<string> EnumerateWorlds(IEnumerable<Feature> features)
        {
            return features
                .Select(x => x.World)
                .Distinct(StringComparer.InvariantCultureIgnoreCase)
                .OrderBy(x => x, StringComparer.InvariantCultureIgnoreCase);
        }

        private static IEnumerable<string> EnumerateCategories(IEnumerable<Feature> features, string world)
        {
            return features
                .Where(x => x.World == world)
                .Select(x => x.Category)
                .Distinct(StringComparer.InvariantCultureIgnoreCase)
                .OrderBy(x => x, StringComparer.InvariantCultureIgnoreCase);
        }

        private static IEnumerable<Feature> FilterFeatures(IEnumerable<Feature> features, string world, string category)
        {
            return features
                .Where(x => x.World == world && x.Category == category)
                .OrderBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase);
        }

        private Bitmap GetRescaledImage(string name, Bitmap img)
        {
            Bitmap rescaledImage;
            if (!this.rescaledImageMap.TryGetValue(name, out rescaledImage))
            {
                rescaledImage = RescaleImage(img);
                this.rescaledImageMap[name] = rescaledImage;
            }

            return rescaledImage;
        }

        private ListViewItem ToItem(Feature s)
        {
            return new ListViewItem(s.Name, this.GetRescaledImage(s.Name, s.Image), s.Name);
        }

        private void UpdateWorlds()
        {
            var featureDb = this.rawFeatures.Value;

            var worlds = EnumerateWorlds(featureDb.EnumerateAll()).ToList();
            var worldsModel = new ComboBoxViewModel(worlds);
            this.worlds.OnNext(worldsModel);
        }

        private void UpdateCategories()
        {
            var featuresDb = this.rawFeatures.Value;
            var worldsModel = this.worlds.Value;

            var categories = EnumerateCategories(featuresDb.EnumerateAll(), worldsModel.SelectedItem).ToList();
            var categoriesModel = new ComboBoxViewModel(categories);
            this.categories.OnNext(categoriesModel);
        }

        private void UpdateFeatures()
        {
            var filteredSections = FilterFeatures(
                this.rawFeatures.Value.EnumerateAll(),
                this.worlds.Value.SelectedItem,
                this.categories.Value.SelectedItem);
            this.features.OnNext(filteredSections.Select(this.ToItem));
        }
    }
}
