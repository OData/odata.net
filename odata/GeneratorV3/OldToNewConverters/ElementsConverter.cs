namespace GeneratorV3.OldToNewConverters
{
    using System.Linq;

    public sealed class ElementsConverter
    {
        private ElementsConverter()
        {
        }

        public static ElementsConverter Instance { get; } = new ElementsConverter();

        public GeneratorV3.Abnf._elements Convert(AbnfParser.CstNodes.Elements elements)
        {
            return new Abnf._elements(
                AlternationConverter.Instance.Convert(elements.Alternation),
                elements.Cwsps.Select(cwsp =>
                    CwspConverter.Instance.Visit(cwsp, default)));
        }
    }
}
