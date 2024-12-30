namespace GeneratorV3.OldToNewConverters
{
    public sealed class CommentConverter
    {
        private CommentConverter()
        {
        }

        public static CommentConverter Instance { get; } = new CommentConverter();

        public GeneratorV3.Abnf._comment Convert(AbnfParser.CstNodes.Comment comment)
        {

        }
    }
}
