namespace GeneratorV3.OldToNewConverters
{
    public sealed class AlternationConverter
    {
        private AlternationConverter()
        {
        }

        public static AlternationConverter Instance { get; } = new AlternationConverter();

        public GeneratorV3.Abnf._alternation Convert(AbnfParser.CstNodes.Alternation alternation)
        {
        }
    }
}
