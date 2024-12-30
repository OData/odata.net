namespace GeneratorV3.OldToNewConverters
{
    public sealed class OptionConverter
    {
        private OptionConverter()
        {
        }

        public static OptionConverter Instance { get; } = new OptionConverter();

        public GeneratorV3.Abnf._option Convert(AbnfParser.CstNodes.Option option)
        {
        }
    }
}
