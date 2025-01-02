namespace GeneratorV3.OldToNewConverters
{
    using System.Linq;

    using AbnfParser.CstNodes;
    using GeneratorV3.OldToNewConverters.Core;

    public sealed class AlternationConverter
    {
        private AlternationConverter()
        {
        }

        public static AlternationConverter Instance { get; } = new AlternationConverter();

        public GeneratorV3.Abnf._alternation Convert(AbnfParser.CstNodes.Alternation alternation)
        {
            return new Abnf._alternation(
                ConcatenationConverter.Instance.Convert(alternation.Concatenation),
                alternation.Inners.Select(inner =>
                    new Abnf.Inners._ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ(
                        InnerConverter.Instance.Convert(inner))));
        }

        private sealed class InnerConverter
        {
            private InnerConverter()
            {
            }

            public static InnerConverter Instance { get; } = new InnerConverter();

            public GeneratorV3.Abnf.Inners._Жcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenation Convert(Alternation.Inner inner)
            {
                return new Abnf.Inners._Жcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenation(
                    inner.PrefixCwsps.Select(cwsp =>
                        CwspConverter.Instance.Visit(cwsp, default)),
                    new Abnf.Inners._ʺx2Fʺ(
                        x2FConverter.Instance.Convert(inner.Slash)),
                    inner.SuffixCwsps.Select(cwsp =>
                        CwspConverter.Instance.Visit(cwsp, default)),
                    ConcatenationConverter.Instance.Convert(inner.Concatenation));
            }
        }
    }
}
