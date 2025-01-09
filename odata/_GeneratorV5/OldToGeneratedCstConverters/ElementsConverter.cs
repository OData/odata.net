namespace _GeneratorV4.OldToGeneratedCstConverters
{
    using System.Linq;

    public sealed class ElementsConverter
    {
        private ElementsConverter()
        {
        }

        public static ElementsConverter Instance { get; } = new ElementsConverter();

        public __Generated.CstNodes.Rules._elements Convert(AbnfParser.CstNodes.Elements elements)
        {
            return new __Generated.CstNodes.Rules._elements(
                AlternationConverter.Instance.Convert(elements.Alternation),
                elements.Cwsps.Select(cwsp =>
                    CwspConverter.Instance.Visit(cwsp, default)));
        }
    }
}
