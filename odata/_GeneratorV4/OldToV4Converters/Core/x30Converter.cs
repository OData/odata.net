namespace _GeneratorV4.OldToV4Converters
{
    using _GeneratorV4.Abnf.CstNodes;

    public sealed class x30Converter
    {
        private x30Converter()
        {
        }

        public static x30Converter Instance { get; } = new x30Converter();

        public Inners._x30 Convert(AbnfParser.CstNodes.Core.x30 x30)
        {
            return Inners._x30.Instance;
        }
    }
}
