namespace _GeneratorV4.OldToV4Converters
{
    using _GeneratorV4.Abnf.CstNodes;

    public sealed class x2EConverter
    {
        private x2EConverter()
        {
        }

        public static x2EConverter Instance { get; } = new x2EConverter();

        public Inners._x2E Convert(AbnfParser.CstNodes.Core.x2E x2E)
        {
            return Inners._x2E.Instance;
        }
    }
}
