namespace GeneratorV3.OldToNewConverters
{
    public sealed class ConcatenationConverter
    {
        private ConcatenationConverter()
        {
        }

        public static ConcatenationConverter Instance { get; } = new ConcatenationConverter();

        public GeneratorV3.Abnf._concatenation Convert(AbnfParser.CstNodes.Concatenation concatenation)
        {
        }
    }
}
