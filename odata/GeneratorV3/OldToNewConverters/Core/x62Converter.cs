namespace GeneratorV3.OldToNewConverters.Core
{
    public sealed class x62Converter
    {
        private x62Converter()
        {
        }

        public static x62Converter Instance { get; } = new x62Converter();

        public GeneratorV3.Abnf.Inners._x62 Convert(AbnfParser.CstNodes.Core.x62 x62)
        {
            return Abnf.Inners._x62.Instance;
        }
    }
}
