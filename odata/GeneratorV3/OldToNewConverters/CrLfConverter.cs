namespace GeneratorV3.OldToNewConverters
{
    using GeneratorV3.OldToNewConverters.Core;

    public sealed class CrLfConverter
    {
        private CrLfConverter()
        {
        }

        public static CrLfConverter Instance { get; } = new CrLfConverter();

        public GeneratorV3.Abnf._CRLF Convert(AbnfParser.CstNodes.Core.Crlf crlf)
        {
            return new Abnf._CRLF(
                CrConverter.Instance.Convert(crlf.Cr),
                LfConverter.Instance.Convert(crlf.Lf));
        }
    }
}
