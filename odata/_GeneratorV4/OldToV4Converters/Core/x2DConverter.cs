namespace _GeneratorV4.OldToV4Converters
{
    using _GeneratorV4.Abnf.CstNodes;

    public sealed class x2DConverter
    {
        private x2DConverter()
        {
        }

        public static x2DConverter Instance { get; } = new x2DConverter();

        public Inners._x2D Convert(AbnfParser.CstNodes.Core.x2D x2D)
        {
            return Inners._x2D.Instance;
        }
    }
}
