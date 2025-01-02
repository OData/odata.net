namespace GeneratorV3.OldToNewConverters.Core
{
    public sealed class LfConverter
    {
        private LfConverter()
        {
        }

        public static LfConverter Instance { get; } = new LfConverter();

        public GeneratorV3.Abnf._LF Convert(AbnfParser.CstNodes.Core.Lf lf)
        {
            return new Abnf._LF(
                new Abnf.Inners._Ⰳx0A(
                    Abnf.Inners._0.Instance,
                    Abnf.Inners._A.Instance));
        }
    }
}
