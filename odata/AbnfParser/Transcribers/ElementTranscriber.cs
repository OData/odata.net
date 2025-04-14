namespace AbnfParser.Transcribers
{
    using System.Text;

    using AbnfParser.CstNodes;
    using Root;

    public sealed class ElementTranscriber : Element.Visitor<Void, StringBuilder>
    {
        private ElementTranscriber()
        {
        }

        public static ElementTranscriber Instance { get; } = new ElementTranscriber();

        protected internal override Void Accept(Element.RuleName node, StringBuilder context)
        {
            RuleNameTranscriber.Instance.Transcribe(node.Value, context);
            return default;
        }

        protected internal override Void Accept(Element.Group node, StringBuilder context)
        {
            GroupTranscriber.Instance.Transcribe(node.Value, context);
            return default;
        }

        protected internal override Void Accept(Element.Option node, StringBuilder context)
        {
            OptionTranscriber.Instance.Transcribe(node.Value, context);
            return default;
        }

        protected internal override Void Accept(Element.CharVal node, StringBuilder context)
        {
            CharValTranscriber.Instance.Transcribe(node.Value, context);
            return default;
        }

        protected internal override Void Accept(Element.NumVal node, StringBuilder context)
        {
            NumValTranscriber.Instance.Visit(node.Value, context);
            return default;
        }

        protected internal override Void Accept(Element.ProseVal node, StringBuilder context)
        {
            ProseValTranscriber.Instance.Visit(node.Value, context);
            return default;
        }
    }
}
