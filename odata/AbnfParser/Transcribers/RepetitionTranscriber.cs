namespace AbnfParser.Transcribers
{
    using System.Text;

    using AbnfParser.CstNodes;
    using Root;

    public sealed class RepetitionTranscriber : Repetition.Visitor<Void, StringBuilder>
    {
        private RepetitionTranscriber()
        {
        }

        public static RepetitionTranscriber Instance { get; } = new RepetitionTranscriber();

        protected internal override Void Accept(Repetition.ElementOnly node, StringBuilder context)
        {
            ElementTranscriber.Instance.Visit(node.Element, context);
            return default;
        }

        protected internal override Void Accept(Repetition.RepeatAndElement node, StringBuilder context)
        {
            RepeatTranscriber.Instance.Visit(node.Repeat, context);
            ElementTranscriber.Instance.Visit(node.Element, context);
            return default;
        }
    }
}
