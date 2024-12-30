namespace GeneratorV3.OldToNewConverters.Core
{
    public sealed class x78Converter
    {
        private x78Converter()
        {
        }

        public static x78Converter Instance { get; } = new x78Converter();

        public GeneratorV3.Abnf.Inners._x78 Convert(AbnfParser.CstNodes.Core.x78 x78)
        {
            return Abnf.Inners._x78.Instance;
        }
    }
}
