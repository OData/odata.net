namespace _GeneratorV5.OldToGeneratedCstConverters
{
    public sealed class LfConverter
    {
        private LfConverter()
        {
        }

        public static LfConverter Instance { get; } = new LfConverter();

        public __Generated.CstNodes.Rules._LF Convert(AbnfParser.CstNodes.Core.Lf lf)
        {
            return new __Generated.CstNodes.Rules._LF(
                new __Generated.CstNodes.Inners._Ⰳx0A(
                    __Generated.CstNodes.Inners._0.Instance,
                    __Generated.CstNodes.Inners._A.Instance));
        }
    }
}
