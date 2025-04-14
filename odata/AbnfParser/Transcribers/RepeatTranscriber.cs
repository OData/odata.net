namespace AbnfParser.Transcribers
{
    using System.Text;

    using AbnfParser.CstNodes;
    using AbnfParser.Transcribers.Core;
    using Root;

    public sealed class RepeatTranscriber : Repeat.Visitor<Void, StringBuilder>
    {
        private RepeatTranscriber()
        {
        }

        public static RepeatTranscriber Instance { get; } = new RepeatTranscriber();

        protected internal override Void Accept(Repeat.Count node, StringBuilder context)
        {
            foreach (var digit in node.Digits)
            {
                DigitTranscriber.Instance.Visit(digit, context);
            }

            return default;
        }

        protected internal override Void Accept(Repeat.Range node, StringBuilder context)
        {
            foreach (var prefixDigit in node.PrefixDigits)
            {
                DigitTranscriber.Instance.Visit(prefixDigit, context);
            }

            x2ATranscriber.Instance.Transcribe(node.Asterisk, context);

            foreach (var suffixDigit in node.SuffixDigits)
            {
                DigitTranscriber.Instance.Visit(suffixDigit, context);
            }

            return default;
        }
    }
}
