using System.Collections.Generic;


namespace GoldBoxExplorer.Lib.Plugins.DaxEcl {

    static public class DaxEclCache {
        static Dictionary<string, DaxEclFile> filesCache = new Dictionary<string, DaxEclFile>();

        static public DaxEclFile GetEclFile(string filename) {
            DaxEclFile fc;
            if (filesCache.TryGetValue(filename, out fc)) {
                return fc;
            }
            fc = new DaxEclFile(filename);
            filesCache.Add(filename, fc);
            return fc;
        }
    }
}
