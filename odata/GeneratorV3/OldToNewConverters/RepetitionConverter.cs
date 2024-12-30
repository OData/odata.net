namespace GeneratorV3.OldToNewConverters
{
    public sealed class RepetitionConverter : AbnfParser.CstNodes.Repetition.Visitor<GeneratorV3.Abnf._repetition, Root.Void>
    {
        private RepetitionConverter()
        {
        }

        public static RepetitionConverter Instance { get; } = new RepetitionConverter();

        protected internal override GeneratorV3.Abnf._repetition Accept(
            AbnfParser.CstNodes.Repetition.ElementOnly node, 
            Root.Void context)
        {
        }

        protected internal override GeneratorV3.Abnf._repetition Accept(
            AbnfParser.CstNodes.Repetition.RepeatAndElement node, 
            Root.Void context)
        {
        }
    }
}
