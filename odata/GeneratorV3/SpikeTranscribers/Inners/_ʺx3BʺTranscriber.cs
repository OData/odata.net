namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _ʺx3BʺTranscriber : ITranscriber<Inners._ʺx3Bʺ>
    {
        private _ʺx3BʺTranscriber()
        {
        }

        public static _ʺx3BʺTranscriber Instance { get; } = new _ʺx3BʺTranscriber();

        public void Transcribe(Inners._ʺx3Bʺ value, StringBuilder builder)
        {
            _x3BTranscriber.Instance.Transcribe(value._x3B_1, builder);
        }
    }
}
