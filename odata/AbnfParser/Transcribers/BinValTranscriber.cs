namespace AbnfParser.Transcribers
{
    using System.Text;

    using AbnfParser.CstNodes;
    using AbnfParser.Transcribers.Core;
    using Root;

    public sealed class BinValTranscriber : BinVal.Visitor<Void, StringBuilder>
    {
        private BinValTranscriber()
        {
        }

        public static BinValTranscriber Instance { get; } = new BinValTranscriber();

        protected internal override Void Accept(BinVal.BitsOnly node, StringBuilder context)
        {
            x62Transcriber.Instance.Transcribe(node.B, context);
            foreach (var bit in node.Bits)
            {
                BitTranscriber.Instance.Visit(bit, context);
            }

            return default;
        }

        protected internal override Void Accept(BinVal.ConcatenatedBits node, StringBuilder context)
        {
            x62Transcriber.Instance.Transcribe(node.B, context);
            foreach (var bit in node.Bits)
            {
                BitTranscriber.Instance.Visit(bit, context);
            }

            foreach (var inner in node.Inners)
            {
                ConcatenatedBitsInnerTranscriber.Instance.Transcribe(inner, context);
            }

            return default;
        }

        public sealed class ConcatenatedBitsInnerTranscriber
        {
            private ConcatenatedBitsInnerTranscriber()
            {
            }

            public static ConcatenatedBitsInnerTranscriber Instance { get; } = new ConcatenatedBitsInnerTranscriber();

            public Void Transcribe(BinVal.ConcatenatedBits.Inner node, StringBuilder context)
            {
                x2ETranscriber.Instance.Transcribe(node.Dot, context);
                foreach (var bit in node.Bits)
                {
                    BitTranscriber.Instance.Visit(bit, context);
                }

                return default;
            }
        }

        protected internal override Void Accept(BinVal.Range node, StringBuilder context)
        {
            x62Transcriber.Instance.Transcribe(node.B, context);
            foreach (var bit in node.Bits)
            {
                BitTranscriber.Instance.Visit(bit, context);
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

            public Void Transcribe(BinVal.Range.Inner node, StringBuilder context)
            {
                x2DTranscriber.Instance.Transcribe(node.Dash, context);
                foreach (var bit in node.Bits)
                {
                    BitTranscriber.Instance.Visit(bit, context);
                }

                return default;
            }
        }
    }
}
