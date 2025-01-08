namespace _GeneratorV4.OldToV4Converters
{
    using _GeneratorV4.Abnf.CstNodes;

    public sealed class CrLfConverter
    {
        private CrLfConverter()
        {
        }

        public static CrLfConverter Instance { get; } = new CrLfConverter();

        public _CRLF Convert(AbnfParser.CstNodes.Core.Crlf crlf)
        {
            return new _CRLF(
                CrConverter.Instance.Convert(crlf.Cr),
                LfConverter.Instance.Convert(crlf.Lf));
        }
    }
}
