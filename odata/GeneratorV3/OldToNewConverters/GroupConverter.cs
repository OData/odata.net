namespace GeneratorV3.OldToNewConverters
{
    public sealed class GroupConverter
    {
        private GroupConverter()
        {
        }

        public static GroupConverter Instance { get; } = new GroupConverter();

        public GeneratorV3.Abnf._group Convert(AbnfParser.CstNodes.Group group)
        {
        }
    }
}
