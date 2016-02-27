namespace Mappy.Services
{
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Forms;

    using Mappy.Data;
    using Mappy.IO;
    using Mappy.Models;

    public class Dispatcher
    {
        private readonly IDialogService dialogService;

        private readonly SectionsService sectionsService;

        private readonly FeatureService featureService;

        public Dispatcher(
            IDialogService dialogService,
            SectionsService sectionsService,
            FeatureService featureService)
        {
            this.dialogService = dialogService;
            this.sectionsService = sectionsService;
            this.featureService = featureService;
        }

        public void Initialize()
        {
            var dlg = this.dialogService.CreateProgressView();
            dlg.Title = "Loading Mappy";
            dlg.ShowProgress = true;
            dlg.CancelEnabled = true;

            var worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += delegate(object sender, DoWorkEventArgs args)
            {
                var w = (BackgroundWorker)sender;

                LoadResult<Section> result;
                if (!SectionLoadingUtils.LoadSections(
                        i => w.ReportProgress((50 * i) / 100),
                        () => w.CancellationPending,
                        out result))
                {
                    args.Cancel = true;
                    return;
                }

                LoadResult<Feature> featureResult;
                if (!FeatureLoadingUtils.LoadFeatures(
                    i => w.ReportProgress(50 + ((50 * i) / 100)),
                    () => w.CancellationPending,
                    out featureResult))
                {
                    args.Cancel = true;
                    return;
                }

                args.Result = new SectionFeatureLoadResult
                {
                    Sections = result.Records,
                    Features = featureResult.Records,
                    Errors = result.Errors
                        .Concat(featureResult.Errors)
                        .GroupBy(x => x.HpiPath)
                        .Select(x => x.First())
                        .ToList(),
                    FileErrors = result.FileErrors
                        .Concat(featureResult.FileErrors)
                        .ToList(),
                };
            };

            worker.ProgressChanged += (sender, args) => dlg.Progress = args.ProgressPercentage;
            worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs args)
            {
                if (args.Error != null)
                {
                    Program.HandleUnexpectedException(args.Error);
                    Application.Exit();
                    return;
                }

                if (args.Cancelled)
                {
                    Application.Exit();
                    return;
                }

                var sectionResult = (SectionFeatureLoadResult)args.Result;

                this.sectionsService.AddSections(sectionResult.Sections);

                foreach (var f in sectionResult.Features)
                {
                    this.featureService.AddFeature(f);
                }

                this.featureService.NotifyChanges();

                if (sectionResult.Errors.Count > 0 || sectionResult.FileErrors.Count > 0)
                {
                    var hpisList = sectionResult.Errors.Select(x => x.HpiPath);
                    var filesList = sectionResult.FileErrors.Select(x => x.HpiPath + "\\" + x.FeaturePath);
                    this.dialogService.ShowError("Failed to load the following files:\n\n"
                        + string.Join("\n", hpisList) + "\n"
                        + string.Join("\n", filesList));
                }

                dlg.Close();
            };

            dlg.CancelPressed += (sender, args) => worker.CancelAsync();

            dlg.MessageText = "Loading sections and features ...";
            worker.RunWorkerAsync();

            dlg.Display();
        }
    }
}
