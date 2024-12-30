namespace GeneratorV3.OldToNewConverters.Core
{
    public sealed class DquoteConverter
    {
        private DquoteConverter()
        {
        }

        public static DquoteConverter Instance { get; } = new DquoteConverter();

        public GeneratorV3.Abnf._DQUOTE Convert(AbnfParser.CstNodes.Core.Dquote dquote)
        {
            return new Abnf._DQUOTE(
                new Abnf.Inners._percentxTWOTWO(
                    Abnf.Inners._TWO.Instance,
                    Abnf.Inners._TWO.Instance));
        }
    }
}
