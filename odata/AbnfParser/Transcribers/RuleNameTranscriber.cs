namespace AbnfParser.Transcribers
{
    using System.Text;

    using AbnfParser.CstNodes;
    using AbnfParser.Transcribers.Core;
    using Root;

    public sealed class RuleNameTranscriber
    {
        private RuleNameTranscriber()
        {
        }

        public static RuleNameTranscriber Instance { get; } = new RuleNameTranscriber();

        public Void Transcribe(RuleName node, StringBuilder context)
        {
            AlphaTranscriber.Instance.Visit(node.Alpha, context);
            foreach (var inner in node.Inners) //// TODO should you compose transcribers like you do parsers and this would be InnerTranscriber.Instance.Many().Transcribe()? this would also normalize `transcribe` vs `visit`
            {
                InnerTranscriber.Instance.Visit(inner, context);
            }

            return default;
        }

        public sealed class InnerTranscriber : RuleName.Inner.Visitor<Void, StringBuilder>
        {
            private InnerTranscriber()
            {
            }

            public static InnerTranscriber Instance { get; } = new InnerTranscriber();

            protected internal override Void Accept(RuleName.Inner.AlphaInner node, StringBuilder context)
            {
                AlphaTranscriber.Instance.Visit(node.Alpha, context);
                return default;
            }

            protected internal override Void Accept(RuleName.Inner.DigitInner node, StringBuilder context)
            {
                DigitTranscriber.Instance.Visit(node.Digit, context);
                return default;
            }

            protected internal override Void Accept(RuleName.Inner.DashInner node, StringBuilder context)
            {
                x2DTranscriber.Instance.Transcribe(node.Dash, context);
                return default;
            }
        }
    }
}
