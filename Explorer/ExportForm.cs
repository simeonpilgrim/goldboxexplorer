using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Windows.Forms;
using GoldBoxExplorer.Lib;
using GoldBoxExplorer.Lib.Plugins;

namespace GoldBoxExplorer
{
    public partial class ExportForm : Form
    {
        private readonly string _filename;
        private readonly IPlugin _plugin;
        private readonly string _FileDirectory;

        public ExportForm()
        {
            InitializeComponent();
        }

        public ExportForm(string currentDirectory, string filename ,IPlugin plugin)
            : this()
        {
            _filename = filename;
            _FileDirectory = currentDirectory;
            _plugin = plugin;
            textBox1.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = textBox1.Text;
            var result = folderBrowserDialog1.ShowDialog();
            if (result.Equals(DialogResult.OK))
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (new WaitCursor()) {
                var targetFolder = textBox1.Text;
                var exportCount = 0;

                var exportFormat = ImageFormat.Png;
                if (formatBmp.Checked) exportFormat = ImageFormat.Bmp;
                else if (formatJpg.Checked) exportFormat = ImageFormat.Jpeg;

                var filename = String.Format(@"{0}\{1}", _FileDirectory, _filename);
                IPlugin plugin = _plugin;

                if (plugin == null) return;
                if (!plugin.IsImageFile()) return;

                int bitmapCounter = 0;

                foreach(var b in plugin.GetBitmapDictionary()) {
                    var blockId = b.Key;
                    var bitmaps = b.Value;

                    foreach(var bitmap in bitmaps) {
                        var outputFilename = string.Format(@"{0}\{1}_{2}_{3}.{4}",
                                                     targetFolder, _filename.TrimEnd(".dax".ToCharArray()),
                                                     blockId, bitmapCounter++, exportFormat.ToString().ToLower());

                        var bm32bpp = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb);

                        using (var g = Graphics.FromImage(bm32bpp)) {
                            g.Clear(Color.FromArgb(103, 247, 159));
                            g.DrawImage(bitmap, 0, 0);
                            bm32bpp.Save(outputFilename, exportFormat);
                        }

                        exportCount++;
                    }
                }

                Hide();

                MessageBox.Show(string.Format("Exported {0} images as {1} to {2}",
                    exportCount, exportFormat.ToString().ToUpper(), targetFolder));
            }
        }
    }
}
