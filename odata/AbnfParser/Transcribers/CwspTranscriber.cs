namespace AbnfParser.Transcribers
{
    using System.Text;

    using AbnfParser.CstNodes;
    using AbnfParser.Transcribers.Core;
    using Root;

    public sealed class CwspTranscriber : Cwsp.Visitor<Void, StringBuilder>
    {
        private CwspTranscriber()
        {
        }

        public static CwspTranscriber Instance { get; } = new CwspTranscriber();

        protected internal override Void Accept(Cwsp.WspOnly node, StringBuilder context)
        {
            WspTranscriber.Instance.Visit(node.Wsp, context);
            return default;
        }

        protected internal override Void Accept(Cwsp.CnlAndWsp node, StringBuilder context)
        {
            CnlTranscriber.Instance.Visit(node.Cnl, context);
            WspTranscriber.Instance.Visit(node.Wsp, context);
            return default;
        }
    }
}
