namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _ʺx29ʺTranscriber : ITranscriber<Inners._ʺx29ʺ>
    {
        private _ʺx29ʺTranscriber()
        {
        }

        public static _ʺx29ʺTranscriber Instance { get; } = new _ʺx29ʺTranscriber();

        public void Transcribe(Inners._ʺx29ʺ value, StringBuilder builder)
        {
            _x29Transcriber.Instance.Transcribe(value._x29_1, builder);
        }
    }
}
