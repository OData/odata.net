﻿namespace _GeneratorV4.OldToV4Converters
{
    using System.Linq;

    using _GeneratorV4.Abnf.CstNodes;
    using AbnfParser.CstNodes;

    public sealed class ConcatenationConverter
    {
        private ConcatenationConverter()
        {
        }

        public static ConcatenationConverter Instance { get; } = new ConcatenationConverter();

        public _concatenation Convert(AbnfParser.CstNodes.Concatenation concatenation)
        {
            return new _concatenation(
                RepetitionConverter.Instance.Visit(concatenation.Repetition, default),
                concatenation.Inners.Select(inner =>
                    InnerConverter.Instance.Convert(inner)));
        }

        private sealed class InnerConverter
        {
            private InnerConverter()
            {
            }

            public static InnerConverter Instance { get; } = new InnerConverter();

            public Inners._Ⲥ1Жcⲻwsp_repetitionↃ Convert(Concatenation.Inner inner)
            {
                return new Inners._Ⲥ1Жcⲻwsp_repetitionↃ(
                    new Inners._1Жcⲻwsp_repetition(
                        inner.Cwsps.Select(cwsp =>
                            CwspConverter.Instance.Visit(cwsp, default)),
                        RepetitionConverter.Instance.Visit(inner.Repetition, default)));
            }
        }
    }
}