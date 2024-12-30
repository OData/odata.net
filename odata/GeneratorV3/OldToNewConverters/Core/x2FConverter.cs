namespace GeneratorV3.OldToNewConverters.Core
{
    public sealed class x2FConverter
    {
        private x2FConverter()
        {
        }

        public static x2FConverter Instance { get; } = new x2FConverter();

        public GeneratorV3.Abnf.Inners._x2F Convert(AbnfParser.CstNodes.Core.x2F x2F)
        {
            return Abnf.Inners._x2F.Instance;
        }
    }
}
