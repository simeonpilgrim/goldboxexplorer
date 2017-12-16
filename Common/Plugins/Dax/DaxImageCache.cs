using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldBoxExplorer.Lib.Plugins.Dax {

    static public class DaxImageCache {
        static Dictionary<string, DaxImageFile> filesCache = new Dictionary<string, DaxImageFile>();

        static public DaxImageFile GetDaxImageFile(string filename) {

            DaxImageFile fc;
            if (filesCache.TryGetValue(filename, out fc)) {
                return fc;
            }
            fc = new DaxImageFile(filename);
            filesCache.Add(filename, fc);
            return fc;
        }
    }
}
