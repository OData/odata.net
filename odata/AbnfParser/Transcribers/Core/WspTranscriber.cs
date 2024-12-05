namespace AbnfParser.Transcribers.Core
{
    using System.Text;

    using AbnfParser.CstNodes.Core;
    using Root;

    public sealed class WspTranscriber : Wsp.Visitor<Void, StringBuilder>
    {
        private WspTranscriber()
        {
        }

        public static WspTranscriber Instance { get; } = new WspTranscriber();

        protected internal override Void Accept(Wsp.Space node, StringBuilder context)
        {
            SpTranscriber.Instance.Transcribe(node.Sp, context);
            return default;
        }

        protected internal override Void Accept(Wsp.Tab node, StringBuilder context)
        {
            HtabTranscriber.Instance.Transcribe(node.Htab, context);
            return default;
        }
    }
}
