namespace AbnfParser.Transcribers
{
    using System.Text;

    using AbnfParser.CstNodes;
    using AbnfParser.Transcribers.Core;
    using Root;

    public sealed class DecValTranscriber : DecVal.Visitor<Void, StringBuilder>
    {
        private DecValTranscriber()
        {
        }

        public static DecValTranscriber Instance { get; } = new DecValTranscriber();

        protected internal override Void Accept(DecVal.DecsOnly node, StringBuilder context)
        {
            x64Transcriber.Instance.Transcribe(node.D, context);
            foreach (var digit in node.Digits)
            {
                DigitTranscriber.Instance.Visit(digit, context);
            }

            return default;
        }

        protected internal override Void Accept(DecVal.ConcatenatedDecs node, StringBuilder context)
        {
            x64Transcriber.Instance.Transcribe(node.D, context);
            foreach (var digit in node.Digits)
            {
                DigitTranscriber.Instance.Visit(digit, context);
            }

            foreach (var inner in node.Inners)
            {
                ConcatenatedDecsInnerTrancriber.Instance.Transcribe(inner, context);
            }

            return default;
        }

        public sealed class ConcatenatedDecsInnerTrancriber
        {
            private ConcatenatedDecsInnerTrancriber()
            {
            }

            public static ConcatenatedDecsInnerTrancriber Instance { get; } = new ConcatenatedDecsInnerTrancriber();

            public Void Transcribe(DecVal.ConcatenatedDecs.Inner node, StringBuilder context)
            {
                x2ETranscriber.Instance.Transcribe(node.Dot, context);
                foreach (var digit in node.Digits)
                {
                    DigitTranscriber.Instance.Visit(digit, context);
                }

                return default;
            }
        }

        protected internal override Void Accept(DecVal.Range node, StringBuilder context)
        {
            x64Transcriber.Instance.Transcribe(node.D, context);
            foreach (var digit in node.Digits)
            {
                DigitTranscriber.Instance.Visit(digit, context);
            }

            foreach (var inner in node.Inners)
            {
                RangeInnerTrancriber.Instance.Transcribe(inner, context);
            }

            return default;
        }

        public sealed class RangeInnerTrancriber
        {
            private RangeInnerTrancriber()
            {
            }

            public static RangeInnerTrancriber Instance { get; } = new RangeInnerTrancriber();

            public Void Transcribe(DecVal.Range.Inner node, StringBuilder context)
            {
                x2DTranscriber.Instance.Transcribe(node.Dash, context);
                foreach (var digit in node.Digits)
                {
                    DigitTranscriber.Instance.Visit(digit, context);
                }

                return default;
            }
        }
    }
}
