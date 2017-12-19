using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GoldBoxExplorer.Lib.Plugins.Dax
{
    internal class DaxWallBlob
    {
        internal List<Bitmap> wallBitmaps;
        internal int blockId;
        internal int blobWidth;
        internal int blobHeight;

        internal DaxWallBlob(List<Bitmap> _wallBitmaps, int _blockId)
        {
            wallBitmaps = _wallBitmaps;
            blockId = _blockId;
            blobWidth = 0;
            blobHeight = 0;

            int idx = 0;
            int x = 0;
            foreach(var wall in wallBitmaps)
            {
                x += wall.Width + DaxWallDefViewer.imageXGap;
                idx++;
                if ((idx % 10) == 0)
                {
                    blobWidth = Math.Max(blobWidth, x);
                    x = 0;
                    blobHeight += DaxWallDefViewer.wallSetHeightPixels;
                }
            }
            if (x > 0)
            {
                blobWidth = Math.Max(blobWidth, x);
                blobHeight += DaxWallDefViewer.wallSetHeightPixels;
            }
        }
    }

    public class DaxWallDefViewer : IGoldBoxViewer
    {
        public event EventHandler<ChangeFileEventArgs> ChangeSelectedFile;
        private const int maxWallHeight = 11;
        internal const int wallsPerSet = 9;
        internal const int wallSetHeight = tilePixels * (maxWallHeight + 1);
        internal const int wallSetHeightPixels = (1 + maxWallHeight) * tilePixels;
        internal const int initialYOffset = 32;
        internal const int imageXGap = 8;
        private const int tilePixels = 8;
        private TabControl tab;
        List<DaxWallBlob> wallSets = new List<DaxWallBlob>();
        int maxWallSetWidth = 0;
        int maxWallSetHeight = 0;

        public DaxWallDefViewer(List<List<Bitmap>> wallBitmaps, List<int> blockIds, float zoom, int containerWidth)
        {
            Zoom = zoom;
            ContainerWidth = containerWidth;
            for (int i = 0; i < wallBitmaps.Count; i++)
            {
                var dwb = new DaxWallBlob(wallBitmaps[i], blockIds[i]);
                wallSets.Add(dwb);
                maxWallSetWidth = Math.Max(maxWallSetWidth, dwb.blobWidth);
                maxWallSetHeight = Math.Max(maxWallSetHeight, dwb.blobHeight);
            }
        }
     
        public float Zoom { get; set; }

        public int ContainerWidth { get; set; }


        void drawWallViews(Graphics surface, DaxWallBlob wallset)
        {
            float x = 0;
            float y = 0;
            int idx = 0;

            foreach (var wall in wallset.wallBitmaps)
            {
                surface.DrawImage(wall, x, y, wall.Width, wall.Height);

                idx++;
                x += wall.Width + imageXGap;

                if ((idx % 10) == 0)
                {
                    x = 0;
                    y += wallSetHeightPixels;
                }
            }
        }

        void InvalidateWalls(object sender, InvalidateEventArgs e)
        {
            var tab = sender as TabControl;
            if (tab == null)
                return;

            foreach (TabPage page in tab.TabPages)
            {
                var pictureBox = page.Controls["wallset"] as PictureBox;
                var dwb = page.Tag as DaxWallBlob;

                pictureBox.Width = (int)(dwb.blobWidth * Zoom);
                pictureBox.Height = (int)(dwb.blobHeight * Zoom);
            }
        }

        public void wallTemplateExportForm(object sender, MouseEventArgs e)
        {
            var page = tab.SelectedTab;
            string tabname = page.Text;
            var dwb = page.Tag as DaxWallBlob;
            using (var form = new WallTemplateExportForm(dwb.wallBitmaps, tabname))
            {
                form.ShowDialog();
            }
        }

        public Control GetControl()
        {
            tab = new TabControl { Dock = DockStyle.Fill };
            tab.Invalidated += InvalidateWalls;

            foreach (var wallset in wallSets)
            {
                var page = new TabPage(wallset.blockId.ToString());
                var pictureBox = new PictureBox();
                page.Tag = wallset;

                page.Size = new Size(wallset.blobWidth, wallset.blobHeight + initialYOffset);
                pictureBox.Image = new Bitmap(wallset.blobWidth, wallset.blobHeight);
                tab.TabPages.Add(page);

                page.AutoScroll = true;
                var exportButton = new Button();
                exportButton.Text = "Export to Wall Template";
                exportButton.AutoSize = true;
                exportButton.MouseClick += wallTemplateExportForm;

                page.Controls.Add(exportButton);
                page.Controls.Add(pictureBox);
                pictureBox.Name = "wallset";
                pictureBox.Location = new Point(0, initialYOffset);
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

                pictureBox.Size = new Size(wallset.blobWidth, wallset.blobHeight);
 
                Graphics surface = Graphics.FromImage(pictureBox.Image);
                drawWallViews(surface, wallset);
            }
            return tab;
        }
    }
}