namespace AbnfParser.Transcribers.Core
{
    using System.Text;

    using AbnfParser.CstNodes.Core;
    using Root;

    public sealed class DigitTranscriber : Digit.Visitor<Void, StringBuilder>
    {
        private DigitTranscriber()
        {
        }

        public static DigitTranscriber Instance { get; } = new DigitTranscriber();

        protected internal override Void Accept(Digit.x30 node, StringBuilder context)
        {
            return x30Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Digit.x31 node, StringBuilder context)
        {
            return x31Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Digit.x32 node, StringBuilder context)
        {
            return x32Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Digit.x33 node, StringBuilder context)
        {
            return x33Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Digit.x34 node, StringBuilder context)
        {
            return x34Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Digit.x35 node, StringBuilder context)
        {
            return x35Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Digit.x36 node, StringBuilder context)
        {
            return x36Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Digit.x37 node, StringBuilder context)
        {
            return x37Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Digit.x38 node, StringBuilder context)
        {
            return x38Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Digit.x39 node, StringBuilder context)
        {
            return x39Transcriber.Instance.Transcribe(node.Value, context);
        }
    }
}
