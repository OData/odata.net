namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;
    using Root;

    public sealed class _ʺx3DʺⳆʺx3Dx2FʺTranscriber : ITranscriber<Inners._ʺx3DʺⳆʺx3Dx2Fʺ>
    {
        private _ʺx3DʺⳆʺx3Dx2FʺTranscriber()
        {
        }

        public static _ʺx3DʺⳆʺx3Dx2FʺTranscriber Instance { get; } = new _ʺx3DʺⳆʺx3Dx2FʺTranscriber();

        public void Transcribe(Inners._ʺx3DʺⳆʺx3Dx2Fʺ value, StringBuilder builder)
        {
        }

        private sealed class Visitor : Inners._ʺx3DʺⳆʺx3Dx2Fʺ.Visitor<Root.Void, StringBuilder>
        {
            private Visitor()
            {
            }

            public static Visitor Instance { get; } = new Visitor();

            protected internal override Void Accept(Inners._ʺx3DʺⳆʺx3Dx2Fʺ._ʺx3Dʺ node, StringBuilder context)
            {
                _ʺx3DʺTranscriber.Instance.Transcribe(node._ʺx3Dʺ_1, context);

                return default;
            }

            protected internal override Void Accept(Inners._ʺx3DʺⳆʺx3Dx2Fʺ._ʺx3Dx2Fʺ node, StringBuilder context)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
