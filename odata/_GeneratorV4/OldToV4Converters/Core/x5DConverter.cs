namespace _GeneratorV4.OldToV4Converters
{
    using _GeneratorV4.Abnf.CstNodes;

    public sealed class x5DConverter
    {
        private x5DConverter()
        {
        }

        public static x5DConverter Instance { get; } = new x5DConverter();

        public Inners._x5D Convert(AbnfParser.CstNodes.Core.x5D x5D)
        {
            return Inners._x5D.Instance;
        }
    }
}
