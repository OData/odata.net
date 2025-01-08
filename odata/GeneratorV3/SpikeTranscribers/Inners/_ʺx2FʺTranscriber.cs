namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _ʺx2FʺTranscriber : ITranscriber<Inners._ʺx2Fʺ>
    {
        private _ʺx2FʺTranscriber()
        {
        }

        public static _ʺx2FʺTranscriber Instance { get; } = new _ʺx2FʺTranscriber();

        public void Transcribe(Inners._ʺx2Fʺ value, StringBuilder builder)
        {
            _x2FTranscriber.Instance.Transcribe(value._x2F_1, builder);
        }
    }
}
