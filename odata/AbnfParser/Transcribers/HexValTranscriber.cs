namespace AbnfParser.Transcribers
{
    using System.Text;

    using AbnfParser.CstNodes;
    using AbnfParser.Transcribers.Core;
    using Root;

    public sealed class HexValTranscriber : HexVal.Visitor<Void, StringBuilder>
    {
        private HexValTranscriber()
        {
        }

        public static HexValTranscriber Instance { get; } = new HexValTranscriber();

        protected internal override Void Accept(HexVal.HexOnly node, StringBuilder context)
        {
            x78Transcriber.Instance.Transcribe(node.X, context);
            foreach (var hexDig in node.HexDigs)
            {
                HexDigTranscriber.Instance.Visit(hexDig, context);
            }

            return default;
        }

        protected internal override Void Accept(HexVal.ConcatenatedHex node, StringBuilder context)
        {
            x78Transcriber.Instance.Transcribe(node.X, context);
            foreach (var hexDig in node.HexDigs)
            {
                HexDigTranscriber.Instance.Visit(hexDig, context);
            }

            foreach (var inner in node.Inners)
            {
                ConcatenatedHexInnerTranscriber.Instance.Transcribe(inner, context);
            }

            return default;
        }

        public sealed class ConcatenatedHexInnerTranscriber
        {
            private ConcatenatedHexInnerTranscriber()
            {
            }

            public static ConcatenatedHexInnerTranscriber Instance { get; } = new ConcatenatedHexInnerTranscriber();

            public Void Transcribe(HexVal.ConcatenatedHex.Inner node, StringBuilder context)
            {
                x2ETranscriber.Instance.Transcribe(node.Dot, context);
                foreach (var hexDig in node.HexDigs)
                {
                    HexDigTranscriber.Instance.Visit(hexDig, context);
                }

                return default;
            }
        }

        protected internal override Void Accept(HexVal.Range node, StringBuilder context)
        {
            x78Transcriber.Instance.Transcribe(node.X, context);
            foreach (var hexDig in node.HexDigs)
            {
                HexDigTranscriber.Instance.Visit(hexDig, context);
            }

            foreach (var inner in node.Inners)
            {
                RangeInnerTranscriber.Instance.Transcribe(inner, context);
            }

            return default;
        }

        public sealed class RangeInnerTranscriber
        {
            private RangeInnerTranscriber()
            {
            }

            public static RangeInnerTranscriber Instance { get; } = new RangeInnerTranscriber();

            public Void Transcribe(HexVal.Range.Inner node, StringBuilder context)
            {
                x2DTranscriber.Instance.Transcribe(node.Dash, context);
                foreach (var hexDig in node.HexDigs)
                {
                    HexDigTranscriber.Instance.Visit(hexDig, context);
                }

                return default;
            }
        }
    }
}
