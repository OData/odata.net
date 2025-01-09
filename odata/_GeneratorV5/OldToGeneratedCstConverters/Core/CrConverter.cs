namespace _GeneratorV4.OldToGeneratedCstConverters
{
    public sealed class CrConverter
    {
        private CrConverter()
        {
        }

        public static CrConverter Instance { get; } = new CrConverter();

        public __Generated.CstNodes.Rules._CR Convert(AbnfParser.CstNodes.Core.Cr cr)
        {
            return new __Generated.CstNodes.Rules._CR(
                new __Generated.CstNodes.Inners._Ⰳx0D(
                    __Generated.CstNodes.Inners._0.Instance,
                    __Generated.CstNodes.Inners._D.Instance));
        }
    }
}
