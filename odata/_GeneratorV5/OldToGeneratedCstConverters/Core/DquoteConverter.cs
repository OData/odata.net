namespace _GeneratorV4.OldToGeneratedCstConverters
{
    public sealed class DquoteConverter
    {
        private DquoteConverter()
        {
        }

        public static DquoteConverter Instance { get; } = new DquoteConverter();

        public __Generated.CstNodes.Rules._DQUOTE Convert(AbnfParser.CstNodes.Core.Dquote dquote)
        {
            return new __Generated.CstNodes.Rules._DQUOTE(
                new __Generated.CstNodes.Inners._Ⰳx22(
                    __Generated.CstNodes.Inners._2.Instance,
                    __Generated.CstNodes.Inners._2.Instance));
        }
    }
}
