namespace _GeneratorV4.OldToGeneratedCstConverters
{
    public sealed class RepetitionConverter : AbnfParser.CstNodes.Repetition.Visitor<__Generated.CstNodes.Rules._repetition, Root.Void>
    {
        private RepetitionConverter()
        {
        }

        public static RepetitionConverter Instance { get; } = new RepetitionConverter();

        protected internal override __Generated.CstNodes.Rules._repetition Accept(
            AbnfParser.CstNodes.Repetition.ElementOnly node, 
            Root.Void context)
        {
            return new __Generated.CstNodes.Rules._repetition(
                null,
                ElementConverter.Instance.Visit(node.Element, context));
        }

        protected internal override __Generated.CstNodes.Rules._repetition Accept(
            AbnfParser.CstNodes.Repetition.RepeatAndElement node, 
            Root.Void context)
        {
            return new __Generated.CstNodes.Rules._repetition(
                RepeatConverter.Instance.Visit(node.Repeat, context),
                ElementConverter.Instance.Visit(node.Element, context));
        }
    }
}
