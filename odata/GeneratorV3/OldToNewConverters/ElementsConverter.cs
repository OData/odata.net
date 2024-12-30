namespace GeneratorV3.OldToNewConverters
{
    public sealed class ElementsConverter
    {
        private ElementsConverter()
        {
        }

        public static ElementsConverter Instance { get; } = new ElementsConverter();

        public GeneratorV3.Abnf._elements Convert(AbnfParser.CstNodes.Elements elements)
        {
        }
    }
}
