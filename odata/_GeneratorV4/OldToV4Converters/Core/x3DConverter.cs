namespace _GeneratorV4.OldToV4Converters
{
    using _GeneratorV4.Abnf.CstNodes;

    public sealed class x3DConverter
    {
        private x3DConverter()
        {
        }

        public static x3DConverter Instance { get; } = new x3DConverter();

        public Inners._x3D Convert(AbnfParser.CstNodes.Core.x3D x3D)
        {
            return Inners._x3D.Instance;
        }
    }
}
