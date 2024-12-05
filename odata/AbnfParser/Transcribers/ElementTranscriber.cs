using AbnfParser.CstNodes;
using Root;
using System.Text;

namespace AbnfParser.Transcribers
{
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
            throw new System.NotImplementedException();
        }

        protected internal override Void Accept(Element.Option node, StringBuilder context)
        {
            throw new System.NotImplementedException();
        }

        protected internal override Void Accept(Element.CharVal node, StringBuilder context)
        {
            throw new System.NotImplementedException();
        }

        protected internal override Void Accept(Element.NumVal node, StringBuilder context)
        {
            throw new System.NotImplementedException();
        }

        protected internal override Void Accept(Element.ProseVal node, StringBuilder context)
        {
            throw new System.NotImplementedException();
        }
    }
}
