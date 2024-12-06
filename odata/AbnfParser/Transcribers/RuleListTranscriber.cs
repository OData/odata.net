namespace AbnfParser.Transcribers
{
    using System.Text;

    using AbnfParser.CstNodes;
    using Root;
    
    public sealed class RuleListTranscriber
    {
        private RuleListTranscriber()
        {
        }

        public static RuleListTranscriber Instance { get; } = new RuleListTranscriber();

        public Void Transcribe(RuleList node, StringBuilder context)
        {
            foreach (var inner in node.Inners)
            {
                InnerTranscriber.Instance.Visit(inner, context);
            }

            return default;
        }

        public sealed class InnerTranscriber : RuleList.Inner.Visitor<Void, StringBuilder>
        {
            private InnerTranscriber()
            {
            }

            public static InnerTranscriber Instance { get; } = new InnerTranscriber();

            protected internal override Void Accept(RuleList.Inner.RuleInner node, StringBuilder context)
            {
                RuleTranscriber.Instance.Transcribe(node.Rule, context);
                return default;
            }

            protected internal override Void Accept(RuleList.Inner.CommentInner node, StringBuilder context)
            {
                foreach (var cwsp in node.Cwsps)
                {
                    CwspTranscriber.Instance.Visit(cwsp, context);
                }

                CnlTranscriber.Instance.Visit(node.Cnl, context);
                return default;
            }
        }
    }
}
