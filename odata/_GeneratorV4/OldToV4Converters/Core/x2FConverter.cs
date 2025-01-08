namespace _GeneratorV4.OldToV4Converters
{
    using _GeneratorV4.Abnf.CstNodes;

    public sealed class x2FConverter
    {
        private x2FConverter()
        {
        }

        public static x2FConverter Instance { get; } = new x2FConverter();

        public Inners._x2F Convert(AbnfParser.CstNodes.Core.x2F x2F)
        {
            return Inners._x2F.Instance;
        }
    }
}
