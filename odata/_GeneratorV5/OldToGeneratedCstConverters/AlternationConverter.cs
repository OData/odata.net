﻿namespace _GeneratorV4.OldToGeneratedCstConverters
{
    using System.Linq;

    using _GeneratorV4.Abnf.CstNodes;

    public sealed class AlternationConverter
    {
        private AlternationConverter()
        {
        }

        public static AlternationConverter Instance { get; } = new AlternationConverter();

        public __Generated.CstNodes.Rules._alternation Convert(AbnfParser.CstNodes.Alternation alternation)
        {
            return new __Generated.CstNodes.Rules._alternation(
                ConcatenationConverter.Instance.Convert(alternation.Concatenation),
                alternation.Inners.Select(inner =>
                    new __Generated.CstNodes.Inners._ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ(
                        InnerConverter.Instance.Convert(inner))));
        }

        private sealed class InnerConverter
        {
            private InnerConverter()
            {
            }

            public static InnerConverter Instance { get; } = new InnerConverter();

            public __Generated.CstNodes.Inners._Жcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenation Convert(AbnfParser.CstNodes.Alternation.Inner inner)
            {
                return new Inners._Жcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenation(
                    inner.PrefixCwsps.Select(cwsp =>
                        CwspConverter.Instance.Visit(cwsp, default)),
                    new __Generated.CstNodes.Inners._ʺx2Fʺ(
                        x2FConverter.Instance.Convert(inner.Slash)),
                    inner.SuffixCwsps.Select(cwsp =>
                        CwspConverter.Instance.Visit(cwsp, default)),
                    ConcatenationConverter.Instance.Convert(inner.Concatenation));
            }
        }
    }
}