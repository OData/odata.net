namespace _GeneratorV4.OldToV4Converters
{
    using _GeneratorV4.Abnf.CstNodes;

    public sealed class x25Converter
    {
        private x25Converter()
        {
        }

        public static x25Converter Instance { get; } = new x25Converter();

        public Inners._x25 Convert(AbnfParser.CstNodes.Core.x25 x25)
        {
            return Inners._x25.Instance;
        }
    }
}
