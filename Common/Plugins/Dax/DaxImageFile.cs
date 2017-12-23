using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GoldBoxExplorer.Lib.Plugins.Dax
{
    public class DaxImageFile : DaxFile
    {
        private readonly List<KeyValuePair<int, IReadOnlyList<Bitmap>>> _bitmaps_dic = new List<KeyValuePair<int, IReadOnlyList<Bitmap>>>();

        public DaxImageFile(string file) : base(file)
        {
            ProcessBlocks();
        }

        protected override sealed void ProcessBlocks()
        {
            foreach (var block in Blocks)
            {
                var renderBlock = new RenderBlockFactory().CreateUsing(block);

                _bitmaps_dic.Add(new KeyValuePair<int, IReadOnlyList<Bitmap>>(block.Id, renderBlock.GetBitmaps().ToList()));
            }
        }

        public IReadOnlyList<KeyValuePair<int, IReadOnlyList<Bitmap>>> GetBitmapDictionary()
        {
            return _bitmaps_dic;
        }
    }
}