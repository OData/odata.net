using AbnfParser.CstNodes.Core;
using Root;
using System.Text;

namespace AbnfParser.Transcribers.Core
{
    public sealed class HexDigTranscriber : HexDig.Visitor<Void, StringBuilder>
    {
        private HexDigTranscriber()
        {
        }

        public static HexDigTranscriber Instance { get; } = new HexDigTranscriber();

        protected internal override Void Accept(HexDig.Digit node, StringBuilder context)
        {
            DigitTranscriber.Instance.Visit(node.Value, context);
            return default;
        }

        protected internal override Void Accept(HexDig.A node, StringBuilder context)
        {
            x41Transcriber.Instance.Transcribe(node.Value, context);
            return default;
        }

        protected internal override Void Accept(HexDig.B node, StringBuilder context)
        {
            x42Transcriber.Instance.Transcribe(node.Value, context);
            return default;
        }

        protected internal override Void Accept(HexDig.C node, StringBuilder context)
        {
            x43Transcriber.Instance.Transcribe(node.Value, context);
            return default;
        }

        protected internal override Void Accept(HexDig.D node, StringBuilder context)
        {
            x44Transcriber.Instance.Transcribe(node.Value, context);
            return default;
        }

        protected internal override Void Accept(HexDig.E node, StringBuilder context)
        {
            x45Transcriber.Instance.Transcribe(node.Value, context);
            return default;
        }

        protected internal override Void Accept(HexDig.F node, StringBuilder context)
        {
            x46Transcriber.Instance.Transcribe(node.Value, context);
            return default;
        }
    }
}
