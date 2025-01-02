namespace GeneratorV3.OldToNewConverters.Core
{
    public sealed class CrConverter
    {
        private CrConverter()
        {
        }

        public static CrConverter Instance { get; } = new CrConverter();

        public GeneratorV3.Abnf._CR Convert(AbnfParser.CstNodes.Core.Cr cr)
        {
            return new Abnf._CR(
                new Abnf.Inners._percentx0D(
                    Abnf.Inners._0.Instance,
                    Abnf.Inners._D.Instance));
        }
    }
}
