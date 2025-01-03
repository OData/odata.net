namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;
    using Root;

    public sealed class WspTranscriber : ITranscriber<_WSP>
    {
        private WspTranscriber()
        {
        }

        public static WspTranscriber Instance { get; } = new WspTranscriber();

        public void Transcribe(_WSP value, StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }

        private sealed class Visitor : _WSP.Visitor<Root.Void, StringBuilder>
        {
            private Visitor()
            {
            }

            public static Visitor Instance { get; } = new Visitor();

            protected internal override Void Accept(_WSP._SP node, StringBuilder context)
            {
                SpTranscriber.Instance.Transcribe(node._SP_1, context);

                return default;
            }

            protected internal override Void Accept(_WSP._HTAB node, StringBuilder context)
            {
                HtabTranscriber.Instance.Transcribe(node._HTAB_1, context);

                return default;
            }
        }
    }
}
