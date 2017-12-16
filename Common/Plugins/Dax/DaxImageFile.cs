using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GoldBoxExplorer.Lib.Plugins.Dax
{
    public class DaxImageFile : DaxFile
    {
        private readonly List<Bitmap> _bitmaps = new List<Bitmap>();
        private readonly List<int> _bitmapIds = new List<int>();
        public DaxImageFile(string file) : base(file)
        {
            ProcessBlocks();
        }

        protected override sealed void ProcessBlocks()
        {
            foreach (var block in Blocks) 
            {
                var renderBlock = new RenderBlockFactory().CreateUsing(block);

                foreach (var bitmap in renderBlock.GetBitmaps())
                {
                    _bitmaps.Add(bitmap);
                    _bitmapIds.Add(renderBlock.getBlockId());
                }
            }
        }
   

        public IList<Bitmap> GetBitmaps()
        {
            return _bitmaps.AsReadOnly();
        }
        public IList<int> GetBitmapIds()
        {
            return _bitmapIds.AsReadOnly();
        }
    }
}