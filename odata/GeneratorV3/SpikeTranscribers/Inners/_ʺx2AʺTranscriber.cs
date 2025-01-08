namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _ʺx2AʺTranscriber : ITranscriber<Inners._ʺx2Aʺ>
    {
        private _ʺx2AʺTranscriber()
        {
        }

        public static _ʺx2AʺTranscriber Instance { get; } = new _ʺx2AʺTranscriber();

        public void Transcribe(Inners._ʺx2Aʺ value, StringBuilder builder)
        {
            _x2ATranscriber.Instance.Transcribe(value._x2A_1, builder);
        }
    }
}
