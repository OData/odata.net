namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;
    using GeneratorV3.SpikeTranscribers.Rules;
    using Root;

    public sealed class _WSPⳆVCHARTranscriber : ITranscriber<Inners._WSPⳆVCHAR>
    {
        private _WSPⳆVCHARTranscriber()
        {
        }

        public static _WSPⳆVCHARTranscriber Instance { get; } = new _WSPⳆVCHARTranscriber();

        public void Transcribe(Inners._WSPⳆVCHAR value, StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }

        private sealed class Visitor : Inners._WSPⳆVCHAR.Visitor<Root.Void, StringBuilder>
        {
            private Visitor()
            {
            }

            public static Visitor Instance { get; } = new Visitor();

            protected internal override Void Accept(Inners._WSPⳆVCHAR._WSP node, StringBuilder context)
            {
                WspTranscriber.Instance.Transcribe(node._WSP_1, context);

                return default;
            }

            protected internal override Void Accept(Inners._WSPⳆVCHAR._VCHAR node, StringBuilder context)
            {
                VcharTranscriber.Instance.Transcribe(node._VCHAR_1, context);

                return default;
            }
        }
    }
}
