using System.IO;

namespace GoldBoxExplorer.Lib.Plugins.Dax
{
    public class DaxFileBlock
    {
        public string FullFileName { get; private set; }
		public string FileNameUpperCase { get; private set; }
        public int Id { get; private set; }
        public byte[] Data { get; set; }

        public DaxFileBlock(string file, int id, byte[] data)
        {
            FullFileName = file;
			FileNameUpperCase = Path.GetFileName(FullFileName).ToUpper();
			Id = id;
            Data = data;
        }
    }
}