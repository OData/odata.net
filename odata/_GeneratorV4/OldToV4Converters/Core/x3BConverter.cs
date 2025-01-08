namespace _GeneratorV4.OldToV4Converters
{
    using _GeneratorV4.Abnf.CstNodes;

    public sealed class x3BConverter
    {
        private x3BConverter()
        {
        }

        public static x3BConverter Instance { get; } = new x3BConverter();

        public Inners._x3B Convert(AbnfParser.CstNodes.Core.x3B x3B)
        {
            return Inners._x3B.Instance;
        }
    }
}
