namespace GeneratorV3.SpikeTranscribers.Inners
{
    using GeneratorV3.Abnf;
    using Root;
    using System.Text;

    public sealed class _WSPⳆCRLF_WSPTranscriber : ITranscriber<Inners._WSPⳆCRLF_WSP>
    {
        private _WSPⳆCRLF_WSPTranscriber()
        {
        }

        public static _WSPⳆCRLF_WSPTranscriber Instance { get; } = new _WSPⳆCRLF_WSPTranscriber();

        public void Transcribe(Inners._WSPⳆCRLF_WSP value, StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }

        private sealed class Visitor : Inners._WSPⳆCRLF_WSP.Visitor<Root.Void, StringBuilder>
        {
            private Visitor()
            {
            }

            public static Visitor Instance { get; } = new Visitor();

            protected internal override Void Accept(Inners._WSPⳆCRLF_WSP._WSP node, StringBuilder context)
            {
                throw new System.NotImplementedException();
            }

            protected internal override Void Accept(Inners._WSPⳆCRLF_WSP._CRLF_WSP node, StringBuilder context)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
