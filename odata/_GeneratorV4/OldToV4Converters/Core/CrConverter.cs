namespace _GeneratorV4.OldToV4Converters
{
    using _GeneratorV4.Abnf.CstNodes;

    public sealed class CrConverter
    {
        private CrConverter()
        {
        }

        public static CrConverter Instance { get; } = new CrConverter();

        public _CR Convert(AbnfParser.CstNodes.Core.Cr cr)
        {
            return new _CR(
                new Inners._Ⰳx0D(
                    Inners._0.Instance,
                    Inners._D.Instance));
        }
    }
}
