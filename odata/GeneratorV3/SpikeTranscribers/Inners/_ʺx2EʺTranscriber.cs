namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _ʺx2EʺTranscriber : ITranscriber<Inners._ʺx2Eʺ>
    {
        private _ʺx2EʺTranscriber()
        {
        }

        public static _ʺx2EʺTranscriber Instance { get; } = new _ʺx2EʺTranscriber();

        public void Transcribe(Inners._ʺx2Eʺ value, StringBuilder builder)
        {
            _x2ETranscriber.Instance.Transcribe(value._x2E_1, builder);
        }
    }
}
