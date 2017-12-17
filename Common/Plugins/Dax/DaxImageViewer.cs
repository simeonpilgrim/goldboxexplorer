using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GoldBoxExplorer.Lib.Plugins.Dax
{
    public class DaxImageViewer : IGoldBoxViewer
    {
        public event EventHandler<ChangeFileEventArgs> ChangeSelectedFile;
        private readonly IReadOnlyDictionary<int, IReadOnlyList<Bitmap>> _bitmaps;

        private readonly bool _display35ImagesPerRow;
        private readonly bool _displayBorder;
        private readonly PictureBox _pictureBox;

        public DaxImageViewer(IReadOnlyDictionary<int, IReadOnlyList<Bitmap>> bitmaps, float zoom, int containerWidth, bool display35ImagesPerRow, bool displayBorder)
        {
            Zoom = zoom;
            ContainerWidth = containerWidth;
            _bitmaps = bitmaps;
            _display35ImagesPerRow = display35ImagesPerRow;
            _displayBorder = displayBorder;
            _pictureBox = new PictureBox();
            _pictureBox.Paint += PictureBoxPaint;
        }

        public float Zoom { get; set; }

        public int ContainerWidth { get; set; }

        public Control GetControl()
        {
            return _pictureBox;
        }

        private void PictureBoxPaint(object sender, PaintEventArgs e)
        {
            var x = 0;
            var y = 0;
            var bitmapCount = _bitmaps.Count;
            var largestImageHeight = 0;
            var pen = new Pen(Color.Fuchsia);
            var pen2 = new Pen(Color.Black);
            int fontSize = (int) (14  * Zoom);
            var font = new Font("Courier New", fontSize);
            var brush = new SolidBrush(Color.FromArgb(85, 85, 85));
            const int MAX_ID_WIDTH = 3;
            int fontWidth = fontSize * MAX_ID_WIDTH;
            int fontHeight = (int)(font.Height);
            int padding = 10;
            int i = 0;
            foreach (var entry in _bitmaps)
            {
                var currentId = entry.Key;

                foreach (var currentImage in entry.Value)
                {
                    int imageWidth = (int)(currentImage.Width * Zoom);
                    int imageHeight = (int)(currentImage.Height * Zoom);

                    largestImageHeight = Math.Max(largestImageHeight, imageHeight);

                    var newRow = false;

                    if (_display35ImagesPerRow)
                    {
                        if (i > 0 && i % 35 == 0)
                            newRow = true;
                    } else if (x + (fontWidth + imageWidth + padding) >= ContainerWidth && i > 0)
                    {
                        newRow = true;
                    }

                    if (newRow)
                    {
                        x = 0;
                        y += Math.Max(largestImageHeight, fontHeight) + padding;
                        largestImageHeight = imageHeight;
                    }

                    e.Graphics.DrawImage(currentImage, x, y, imageWidth, imageHeight);
                    e.Graphics.DrawString(currentId.ToString(), font, brush, x + imageWidth, y);
                    if (_displayBorder)
                    {
                        e.Graphics.DrawRectangle(pen, x, y, imageWidth, imageHeight);
                    }
                    //var ox = x;

                    x += imageWidth + fontWidth + padding;
                    //e.Graphics.DrawRectangle(pen2, ox, y, imageWidth + fontWidth, Math.Max(largestImageHeight, fontHeight));
                    i++;
                }
            }

            _pictureBox.Width = ContainerWidth;
            _pictureBox.Height = (int) (fontSize + y + (largestImageHeight*Zoom));
        }
    }
}