namespace GeneratorV3.OldToNewConverters
{
    public sealed class CrLfConverter
    {
        private CrLfConverter()
        {
        }

        public static CrLfConverter Instance { get; } = new CrLfConverter();

        public GeneratorV3.Abnf._CRLF Convert(AbnfParser.CstNodes.Core.Crlf crlf)
        {
        }
    }
}
