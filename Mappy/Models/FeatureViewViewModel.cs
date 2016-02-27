namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using Mappy.Data;
    using Mappy.Services;

    public class FeatureViewViewModel : ISectionViewViewModel
    {
        private readonly BehaviorSubject<ComboBoxViewModel> worlds = new BehaviorSubject<ComboBoxViewModel>(ComboBoxViewModel.Empty);

        private readonly BehaviorSubject<ComboBoxViewModel> categories = new BehaviorSubject<ComboBoxViewModel>(ComboBoxViewModel.Empty);

        private readonly Subject<int> selectWorldEvent = new Subject<int>();

        private readonly Subject<int> selectCategoryEvent = new Subject<int>();

        private readonly BehaviorSubject<IEnumerable<ListViewItem>> features = new BehaviorSubject<IEnumerable<ListViewItem>>(Enumerable.Empty<ListViewItem>());

        private readonly Dictionary<string, Bitmap> rescaledImageMap = new Dictionary<string, Bitmap>();

        private readonly FeatureService featureService;

        private readonly ISubject<bool> worldsInvalidated = new Subject<bool>();
        private readonly ISubject<bool> categoriesInvalidated = new Subject<bool>();
        private readonly ISubject<bool> featuresInvalidated = new Subject<bool>();

        public FeatureViewViewModel(CoreModel model, FeatureService featureService)
        {
            featureService.FeaturesChanged += this.OnFeaturesChanged;

            this.selectWorldEvent
                .Select(i => this.worlds.Value.Select(i))
                .Subscribe(this.worlds);

            this.selectCategoryEvent
                .Select(i => this.categories.Value.Select(i))
                .Subscribe(this.categories);

            this.worlds.Select(_ => true).Subscribe(this.categoriesInvalidated);
            this.categories.Select(_ => true).Subscribe(this.featuresInvalidated);

            this.worldsInvalidated
                .DistinctUntilChanged()
                .Where(x => x)
                .Subscribe(_ => this.UpdateWorlds());

            this.categoriesInvalidated
                .DistinctUntilChanged()
                .Where(x => x)
                .Subscribe(_ => this.UpdateCategories());

            this.featuresInvalidated
                .DistinctUntilChanged()
                .Where(x => x)
                .Subscribe(_ => this.UpdateFeatures());

            this.featureService = featureService;
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
            this.worldsInvalidated.OnNext(false);

            var worlds = this.featureService.EnumerateWorlds();
            var worldsModel = new ComboBoxViewModel(worlds.ToList());
            this.worlds.OnNext(worldsModel);
        }

        private void UpdateCategories()
        {
            this.categoriesInvalidated.OnNext(false);

            var world = this.worlds.Value.SelectedItem;
            var categories = this.featureService.EnumerateCategories(world);
            var categoriesModel = new ComboBoxViewModel(categories.ToList());
            this.categories.OnNext(categoriesModel);
        }

        private void UpdateFeatures()
        {
            this.featuresInvalidated.OnNext(false);

            var world = this.worlds.Value.SelectedItem;
            var category = this.categories.Value.SelectedItem;
            var features = this.featureService.EnumerateFeatures(world, category);
            this.features.OnNext(features.Select(this.ToItem).ToList());
        }

        private void OnFeaturesChanged(object sender, EventArgs e)
        {
            // this will end up invalidating everything
            // via knock-on effect
            this.worldsInvalidated.OnNext(true);
        }
    }
}
