namespace AbnfParser.Transcribers
{
    using System.Text;

    using AbnfParser.CstNodes;
    using AbnfParser.Transcribers.Core;
    using Root;

    public sealed class NumValTranscriber : NumVal.Visitor<Void, StringBuilder>
    {
        private NumValTranscriber()
        {
        }

        public static NumValTranscriber Instance { get; } = new NumValTranscriber();

        protected internal override Void Accept(NumVal.BinVal node, StringBuilder context)
        {
            x25Transcriber.Instance.Transcribe(node.Percent, context);
            BinValTranscriber.Instance.Visit(node.Value, context);
            return default;
        }

        protected internal override Void Accept(NumVal.DecVal node, StringBuilder context)
        {
            x25Transcriber.Instance.Transcribe(node.Percent, context);
            DecValTranscriber.Instance.Visit(node.Value, context);
            return default;
        }

        protected internal override Void Accept(NumVal.HexVal node, StringBuilder context)
        {
            x25Transcriber.Instance.Transcribe(node.Percent, context);
            HexValTranscriber.Instance.Visit(node.Value, context);
            return default;
        }
    }
}
