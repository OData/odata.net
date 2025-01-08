namespace _GeneratorV4.OldToV4Converters
{
    using System.Linq;

    using _GeneratorV4.Abnf.CstNodes;
    using AbnfParser.CstNodes;

    public sealed class AlternationConverter
    {
        private AlternationConverter()
        {
        }

        public static AlternationConverter Instance { get; } = new AlternationConverter();

        public _alternation Convert(AbnfParser.CstNodes.Alternation alternation)
        {
            return new _alternation(
                ConcatenationConverter.Instance.Convert(alternation.Concatenation),
                alternation.Inners.Select(inner =>
                    new Inners._ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ(
                        InnerConverter.Instance.Convert(inner))));
        }

        private sealed class InnerConverter
        {
            private InnerConverter()
            {
            }

            public static InnerConverter Instance { get; } = new InnerConverter();

            public Inners._Жcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenation Convert(Alternation.Inner inner)
            {
                return new Inners._Жcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenation(
                    inner.PrefixCwsps.Select(cwsp =>
                        CwspConverter.Instance.Visit(cwsp, default)),
                    new Inners._ʺx2Fʺ(
                        x2FConverter.Instance.Convert(inner.Slash)),
                    inner.SuffixCwsps.Select(cwsp =>
                        CwspConverter.Instance.Visit(cwsp, default)),
                    ConcatenationConverter.Instance.Convert(inner.Concatenation));
            }
        }
    }
}
