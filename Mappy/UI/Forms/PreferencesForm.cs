namespace Mappy.UI.Forms
{
    using System;
    using System.Windows.Forms;

    using Ookii.Dialogs;

    public partial class PreferencesForm : Form
    {
        public PreferencesForm()
        {
            this.InitializeComponent();
        }

        private void PreferencesFormLoad(object sender, EventArgs e)
        {
            if (MappySettings.Settings.SearchPaths != null)
            {
                foreach (string dir in MappySettings.Settings.SearchPaths)
                {
                    ListViewItem i = new ListViewItem(dir);
                    this.searchPathsListView.Items.Add(i);
                }
            }
        }

        private void AddButtonClick(object sender, EventArgs e)
        {
            var d = new VistaFolderBrowserDialog();
            DialogResult r = d.ShowDialog(this);
            if (r == DialogResult.OK)
            {
                ListViewItem i = new ListViewItem(d.SelectedPath);
                this.searchPathsListView.Items.Add(i);
                i.Selected = true;
            }

            this.searchPathsListView.Focus();
        }

        private void RemoveButtonClick(object sender, EventArgs e)
        {
            var selectedIndices = this.searchPathsListView.SelectedIndices;
            if (selectedIndices.Count > 0)
            {
                var i = selectedIndices[0];
                this.searchPathsListView.Items.RemoveAt(i);

                // select the item before the one we removed,
                // as long as there are items remaining in the list.
                if (this.searchPathsListView.Items.Count > 0)
                {
                    this.searchPathsListView.Items[Math.Max(i - 1, 0)].Selected = true;
                    this.searchPathsListView.Focus();
                }
            }
        }

        private void UpButtonClick(object sender, EventArgs e)
        {
            foreach (int i in this.searchPathsListView.SelectedIndices)
            {
                if (i == 0)
                {
                    this.searchPathsListView.Focus();
                    continue;
                }

                ListViewItem tmp = this.searchPathsListView.Items[i];
                this.searchPathsListView.Items.RemoveAt(i);
                this.searchPathsListView.Items.Insert(i - 1, tmp);

                this.searchPathsListView.Items[i - 1].Selected = true;
                this.searchPathsListView.Focus();
            }
        }

        private void DownButtonClick(object sender, EventArgs e)
        {
            foreach (int i in this.searchPathsListView.SelectedIndices)
            {
                if (i == this.searchPathsListView.Items.Count - 1)
                {
                    this.searchPathsListView.Focus();
                    continue;
                }

                ListViewItem tmp = this.searchPathsListView.Items[i];
                this.searchPathsListView.Items.RemoveAt(i);
                this.searchPathsListView.Items.Insert(i + 1, tmp);

                this.searchPathsListView.Items[i + 1].Selected = true;
                this.searchPathsListView.Focus();
            }
        }

        private void OkButtonClick(object sender, EventArgs e)
        {
            System.Collections.Specialized.StringCollection s = new System.Collections.Specialized.StringCollection();
            foreach (ListViewItem i in this.searchPathsListView.Items)
            {
                s.Add(i.Text);
            }

            MappySettings.Settings.SearchPaths = s;
            MappySettings.SaveSettings();
        }
    }
}
