using System.IO;

namespace GoldBoxExplorer.Lib.Plugins.Dax
{
    public class MonoBlockSpecification : IFileBlockSpecification
    {
        public bool IsSatisfiedBy(DaxFileBlock block)
        {
            return (block.Data.Length%8) == 0 &&
                   (block.FileNameUpperCase.StartsWith("8X8"));
        }
    }
}