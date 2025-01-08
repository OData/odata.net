namespace _GeneratorV4.OldToV4Converters
{
    using _GeneratorV4.Abnf.CstNodes;

    public sealed class x2AConverter
    {
        private x2AConverter()
        {
        }

        public static x2AConverter Instance { get; } = new x2AConverter();

        public Inners._x2A Convert(AbnfParser.CstNodes.Core.x2A x2A)
        {
            return Inners._x2A.Instance;
        }
    }
}
