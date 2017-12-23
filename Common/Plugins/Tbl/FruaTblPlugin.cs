using System.IO;
using System.Drawing;
using System.Collections.Generic;

namespace GoldBoxExplorer.Lib.Plugins.Tbl
{
    public class FruaTblPlugin : IPlugin
    {
        private FruaTlbFile _file;
        public bool IsSatisifedBy(string path)
        {
            var filename = Path.GetFileName(path);
            return filename != null && filename.ToUpper().EndsWith(".TLB");
        }

        public IPlugin CreateUsing(PluginParameter args)
        {
            _file = new FruaTlbFile(args.Filename);
            Viewer = new FruaTlbViewer(_file, args);
            return this;
        }

        public IGoldBoxViewer Viewer { get; set; }

        public bool IsImageFile() { return true; }

        public IReadOnlyList<KeyValuePair<int, IReadOnlyList<Bitmap>>> GetBitmapDictionary() {
            return null;
        }
    }
}