namespace GeneratorV3.OldToNewConverters.Core
{
    public sealed class x3BConverter
    {
        private x3BConverter()
        {
        }

        public static x3BConverter Instance { get; } = new x3BConverter();

        public Abnf.Inners._x3B Convert(AbnfParser.CstNodes.Core.x3B x3B)
        {
            return Abnf.Inners._x3B.Instance;
        }
    }
}
