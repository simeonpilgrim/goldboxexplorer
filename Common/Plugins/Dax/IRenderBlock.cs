using System.Collections.Generic;
using System.Drawing;

namespace GoldBoxExplorer.Lib.Plugins.Dax
{
    public interface IRenderBlock
    {
        void setBlockId(int id);

        IEnumerable<Bitmap> GetBitmaps();
    }
}