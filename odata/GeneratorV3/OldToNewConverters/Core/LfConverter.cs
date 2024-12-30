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
                new Abnf.Inners._percentxZEROA(
                    Abnf.Inners._ZERO.Instance,
                    Abnf.Inners._A.Instance));
        }
    }
}
