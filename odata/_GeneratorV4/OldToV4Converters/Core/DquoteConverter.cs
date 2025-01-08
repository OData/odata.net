namespace _GeneratorV4.OldToV4Converters
{
    using _GeneratorV4.Abnf.CstNodes;

    public sealed class DquoteConverter
    {
        private DquoteConverter()
        {
        }

        public static DquoteConverter Instance { get; } = new DquoteConverter();

        public _DQUOTE Convert(AbnfParser.CstNodes.Core.Dquote dquote)
        {
            return new _DQUOTE(
                new Inners._Ⰳx22(
                    Inners._2.Instance,
                    Inners._2.Instance));
        }
    }
}
