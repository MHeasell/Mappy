namespace Mappy.UI.Controls
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Mappy.Util;

    public partial class StartPositionsView : UserControl
    {
        public StartPositionsView()
        {
            this.InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            this.PopulateList();
        }

        private void PopulateList()
        {
            ImageList im = new ImageList();
            im.ImageSize = new Size(64, 64);

            for (int i = 0; i < 10; i++)
            {
                im.Images.Add(Util.GetStartImage(i + 1));
            }

            this.listView1.LargeImageList = im;

            for (int i = 0; i < 10; i++)
            {
                var item = new ListViewItem("Position " + (i + 1), i);
                item.Tag = new StartPositionDragData(i);
                this.listView1.Items.Add(item);
            }
        }

        private void ListView1ItemDrag(object sender, ItemDragEventArgs e)
        {
            ListViewItem item = (ListViewItem)e.Item;
            this.listView1.DoDragDrop(item.Tag, DragDropEffects.Copy);
        }
    }
}
