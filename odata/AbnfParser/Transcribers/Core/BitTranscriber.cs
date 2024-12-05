namespace AbnfParser.Transcribers.Core
{
    using System.Text;

    using AbnfParser.CstNodes.Core;
    using Root;

    public sealed class BitTranscriber : Bit.Visitor<Void, StringBuilder>
    {
        private BitTranscriber()
        {
        }

        public static BitTranscriber Instance { get; } = new BitTranscriber();

        protected internal override Void Accept(Bit.Zero node, StringBuilder context)
        {
            return x30Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Bit.One node, StringBuilder context)
        {
            return x31Transcriber.Instance.Transcribe(node.Value, context);
        }
    }
}
