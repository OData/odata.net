namespace _GeneratorV4.OldToV4Converters
{
    using System.Linq;

    using _GeneratorV4.Abnf.CstNodes;

    public sealed class ElementsConverter
    {
        private ElementsConverter()
        {
        }

        public static ElementsConverter Instance { get; } = new ElementsConverter();

        public _elements Convert(AbnfParser.CstNodes.Elements elements)
        {
            return new _elements(
                AlternationConverter.Instance.Convert(elements.Alternation),
                elements.Cwsps.Select(cwsp =>
                    CwspConverter.Instance.Visit(cwsp, default)));
        }
    }
}
