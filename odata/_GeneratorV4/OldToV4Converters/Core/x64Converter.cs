namespace _GeneratorV4.OldToV4Converters
{
    using _GeneratorV4.Abnf.CstNodes;

    public sealed class x64Converter
    {
        private x64Converter()
        {
        }

        public static x64Converter Instance { get; } = new x64Converter();

        public Inners._x64 Convert(AbnfParser.CstNodes.Core.x64 x64)
        {
            return Inners._x64.Instance;
        }
    }
}
