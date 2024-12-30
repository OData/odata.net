namespace GeneratorV3.OldToNewConverters.Core
{
    public sealed class x2DConverter
    {
        private x2DConverter()
        {
        }

        public static x2DConverter Instance { get; } = new x2DConverter();

        public GeneratorV3.Abnf.Inners._x2D Convert(AbnfParser.CstNodes.Core.x2D x2D)
        {
            return Abnf.Inners._x2D.Instance;
        }
    }
}
