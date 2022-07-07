﻿namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using Mappy.Data;
    using Mappy.Services;

    public sealed class FeatureViewViewModel : ISectionViewViewModel, IDisposable
    {
        private readonly BehaviorSubject<ComboBoxViewModel> worlds = new BehaviorSubject<ComboBoxViewModel>(ComboBoxViewModel.Empty);
        private readonly BehaviorSubject<ComboBoxViewModel> categories = new BehaviorSubject<ComboBoxViewModel>(ComboBoxViewModel.Empty);
        private readonly BehaviorSubject<IEnumerable<ListViewItem>> features = new BehaviorSubject<IEnumerable<ListViewItem>>(Enumerable.Empty<ListViewItem>());

        private readonly Subject<bool> worldsInvalidated = new Subject<bool>();
        private readonly Subject<bool> categoriesInvalidated = new Subject<bool>();
        private readonly Subject<bool> featuresInvalidated = new Subject<bool>();

        private readonly Subject<int> selectWorldEvent = new Subject<int>();
        private readonly Subject<int> selectCategoryEvent = new Subject<int>();

        private readonly Dictionary<string, Bitmap> rescaledImageMap = new Dictionary<string, Bitmap>();

        private readonly FeatureService featureService;

        private readonly Dispatcher dispatcher;

        public FeatureViewViewModel(FeatureService featureService, Dispatcher dispatcher)
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
            this.dispatcher = dispatcher;
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

        public void SetSelectedItem(string featureName)
        {
            this.dispatcher.SetSelectedFeature(featureName);
        }

        public void Dispose()
        {
            this.worlds.Dispose();
            this.categories.Dispose();
            this.features.Dispose();

            this.selectWorldEvent.Dispose();
            this.selectCategoryEvent.Dispose();

            this.worldsInvalidated.Dispose();
            this.categoriesInvalidated.Dispose();
            this.featuresInvalidated.Dispose();
        }

        private static Bitmap RescaleImage(Bitmap img)
        {
            var outWidth = 64;
            var outHeight = 64;

            var thumb = new Bitmap(outWidth, outHeight);
            var g = Graphics.FromImage(thumb);

            var ratioX = outWidth / (double)img.Width;
            var ratioY = outHeight / (double)img.Height;

            // use the smaller ratio
            var ratio = Math.Min(ratioX, ratioY);

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

            var posX = (outWidth - newWidth) / 2;
            var posY = (outHeight - newHeight) / 2;

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
