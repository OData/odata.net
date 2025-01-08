namespace _GeneratorV4.OldToV4Converters
{
    using _GeneratorV4.Abnf.CstNodes;

    public sealed class RepetitionConverter : AbnfParser.CstNodes.Repetition.Visitor<_repetition, Root.Void>
    {
        private RepetitionConverter()
        {
        }

        public static RepetitionConverter Instance { get; } = new RepetitionConverter();

        protected internal override _repetition Accept(
            AbnfParser.CstNodes.Repetition.ElementOnly node, 
            Root.Void context)
        {
            return new _repetition(
                null,
                ElementConverter.Instance.Visit(node.Element, context));
        }

        protected internal override _repetition Accept(
            AbnfParser.CstNodes.Repetition.RepeatAndElement node, 
            Root.Void context)
        {
            return new _repetition(
                RepeatConverter.Instance.Visit(node.Repeat, context),
                ElementConverter.Instance.Visit(node.Element, context));
        }
    }
}
