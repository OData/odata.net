namespace GeneratorV3.OldToNewConverters.Core
{
    public sealed class x5DConverter
    {
        private x5DConverter()
        {
        }

        public static x5DConverter Instance { get; } = new x5DConverter();

        public GeneratorV3.Abnf.Inners._x5D Convert(AbnfParser.CstNodes.Core.x5D x5D)
        {
            return Abnf.Inners._x5D.Instance;
        }
    }
}
