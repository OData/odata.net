namespace _GeneratorV4.OldToV4Converters
{
    using _GeneratorV4.Abnf.CstNodes;

    public sealed class x5BConverter
    {
        private x5BConverter()
        {
        }

        public static x5BConverter Instance { get; } = new x5BConverter();

        public Inners._x5B Convert(AbnfParser.CstNodes.Core.x5B x5B)
        {
            return Inners._x5B.Instance;
        }
    }
}
