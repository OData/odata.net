namespace _GeneratorV4.OldToGeneratedCstConverters
{
    public sealed class CrLfConverter
    {
        private CrLfConverter()
        {
        }

        public static CrLfConverter Instance { get; } = new CrLfConverter();

        public __Generated.CstNodes.Rules._CRLF Convert(AbnfParser.CstNodes.Core.Crlf crlf)
        {
            return new __Generated.CstNodes.Rules._CRLF(
                CrConverter.Instance.Convert(crlf.Cr),
                LfConverter.Instance.Convert(crlf.Lf));
        }
    }
}
