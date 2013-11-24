namespace Mappy.UI.Forms
{
    using System;
    using System.Windows.Forms;

    public partial class PreferencesForm : Form
    {
        public PreferencesForm()
        {
            this.InitializeComponent();
        }

        private void PreferencesFormLoad(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.SearchDirectories != null)
            {
                foreach (string dir in Properties.Settings.Default.SearchDirectories)
                {
                    ListViewItem i = new ListViewItem(dir);
                    this.listView1.Items.Add(i);
                }
            }
        }

        private void Button1Click(object sender, EventArgs e)
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
            DialogResult r = d.ShowDialog(this);
            if (r == DialogResult.OK)
            {
                ListViewItem i = new ListViewItem(d.SelectedPath);
                this.listView1.Items.Add(i);
                i.Selected = true;
            }

            this.listView1.Focus();
        }

        private void Button2Click(object sender, EventArgs e)
        {
            foreach (int i in this.listView1.SelectedIndices)
            {
                this.listView1.Items.RemoveAt(i);
            }
        }

        private void Button3Click(object sender, EventArgs e)
        {
            foreach (int i in this.listView1.SelectedIndices)
            {
                if (i == 0)
                {
                    this.listView1.Focus();
                    continue;
                }

                ListViewItem tmp = this.listView1.Items[i];
                this.listView1.Items.RemoveAt(i);
                this.listView1.Items.Insert(i - 1, tmp);

                this.listView1.Items[i - 1].Selected = true;
                this.listView1.Focus();
            }
        }

        private void Button4Click(object sender, EventArgs e)
        {
            foreach (int i in this.listView1.SelectedIndices)
            {
                if (i == this.listView1.Items.Count - 1)
                {
                    this.listView1.Focus();
                    continue;
                }

                ListViewItem tmp = this.listView1.Items[i];
                this.listView1.Items.RemoveAt(i);
                this.listView1.Items.Insert(i + 1, tmp);

                this.listView1.Items[i + 1].Selected = true;
                this.listView1.Focus();
            }
        }

        private void Button5Click(object sender, EventArgs e)
        {
            System.Collections.Specialized.StringCollection s = new System.Collections.Specialized.StringCollection();
            foreach (ListViewItem i in this.listView1.Items)
            {
                s.Add(i.Text);
            }

            Properties.Settings.Default.SearchDirectories = s;
            Properties.Settings.Default.Save();
        }
    }
}
