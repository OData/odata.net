namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _ʺx25ʺTranscriber : ITranscriber<Inners._ʺx25ʺ>
    {
        private _ʺx25ʺTranscriber()
        {
        }

        public static _ʺx25ʺTranscriber Instance { get; } = new _ʺx25ʺTranscriber();

        public void Transcribe(Inners._ʺx25ʺ value, StringBuilder builder)
        {
            _x25Transcriber.Instance.Transcribe(value._x25_1, builder);
        }
    }
}
