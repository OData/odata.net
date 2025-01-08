namespace _GeneratorV4.OldToV4Converters
{
    using _GeneratorV4.Abnf.CstNodes;

    public sealed class LfConverter
    {
        private LfConverter()
        {
        }

        public static LfConverter Instance { get; } = new LfConverter();

        public _LF Convert(AbnfParser.CstNodes.Core.Lf lf)
        {
            return new _LF(
                new Inners._Ⰳx0A(
                    Inners._0.Instance,
                    Inners._A.Instance));
        }
    }
}
