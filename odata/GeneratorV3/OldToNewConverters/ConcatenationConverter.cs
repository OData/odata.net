namespace GeneratorV3.OldToNewConverters
{
    using System.Linq;

    using AbnfParser.CstNodes;

    public sealed class ConcatenationConverter
    {
        private ConcatenationConverter()
        {
        }

        public static ConcatenationConverter Instance { get; } = new ConcatenationConverter();

        public GeneratorV3.Abnf._concatenation Convert(AbnfParser.CstNodes.Concatenation concatenation)
        {
            return new Abnf._concatenation(
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

            public GeneratorV3.Abnf.Inners._openONEasteriskcⲻwsp_repetitionↃ Convert(Concatenation.Inner inner)
            {
                return new Abnf.Inners._openONEasteriskcⲻwsp_repetitionↃ(
                    new Abnf.Inners._ONEasteriskcⲻwsp_repetition(
                        inner.Cwsps.Select(cwsp =>
                            CwspConverter.Instance.Visit(cwsp, default)),
                        RepetitionConverter.Instance.Visit(inner.Repetition, default)));
            }
        }
    }
}
