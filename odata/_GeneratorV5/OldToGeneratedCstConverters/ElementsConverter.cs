namespace _GeneratorV5.OldToGeneratedCstConverters
{
    using _GeneratorV5.ManualParsers.Rules;
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
                    CwspConverter.Instance.Visit(cwsp, default)).Convert());
        }
    }
}
