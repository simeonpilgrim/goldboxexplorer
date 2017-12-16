namespace GoldBoxExplorer.Lib.Plugins.Dax
{
    public class RenderBlockFactory
    {
        public IRenderBlock CreateUsing(DaxFileBlock block)
        {
            if (new EgaSpriteBlockSpecification().IsSatisfiedBy(block))
                return new EgaSpriteBlock(block);

            if (new VgaStrataBlockSpecification().IsSatisfiedBy(block))
                return new VgaStrataBlock(block);

            if (new VgaSpriteBlockSpecification().IsSatisfiedBy(block))
                return new VgaSpriteBlock(block);

            if (new VgaMixedBlockSpecification().IsSatisfiedBy(block))
                return new VgaMixedBlock(block);

            if (new VgaBlockSpecification().IsSatisfiedBy(block))
                return new VgaBlock(block);

            if (new EgaBlockSpecification().IsSatisfiedBy(block))
                return new EgaBlock(block);

            if (new MonoBlockSpecification().IsSatisfiedBy(block))
                return new MonoBlock(block);

            return new UnknownBlock();
        }
    }
}