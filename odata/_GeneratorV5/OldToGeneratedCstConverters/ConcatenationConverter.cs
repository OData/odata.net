namespace _GeneratorV5.OldToGeneratedCstConverters
{
    using _GeneratorV5.ManualParsers.Rules;
    using System.Linq;

    public sealed class ConcatenationConverter
    {
        private ConcatenationConverter()
        {
        }

        public static ConcatenationConverter Instance { get; } = new ConcatenationConverter();

        public __Generated.CstNodes.Rules._concatenation Convert(AbnfParser.CstNodes.Concatenation concatenation)
        {
            return new __Generated.CstNodes.Rules._concatenation(
                RepetitionConverter.Instance.Visit(concatenation.Repetition, default),
                concatenation.Inners.Select(inner =>
                    InnerConverter.Instance.Convert(inner)).Convert());
        }

        private sealed class InnerConverter
        {
            private InnerConverter()
            {
            }

            public static InnerConverter Instance { get; } = new InnerConverter();

            public __Generated.CstNodes.Inners._Ⲥ1Жcⲻwsp_repetitionↃ Convert(AbnfParser.CstNodes.Concatenation.Inner inner)
            {
                return new __Generated.CstNodes.Inners._Ⲥ1Жcⲻwsp_repetitionↃ(
                    new __Generated.CstNodes.Inners._1Жcⲻwsp_repetition(
                        inner.Cwsps.Select(cwsp =>
                            CwspConverter.Instance.Visit(cwsp, default)).Convert2(),
                        RepetitionConverter.Instance.Visit(inner.Repetition, default)));
            }
        }
    }
}
