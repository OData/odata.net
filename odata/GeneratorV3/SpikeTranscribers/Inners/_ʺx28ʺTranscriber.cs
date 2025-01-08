namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _ʺx28ʺTranscriber : ITranscriber<Inners._ʺx28ʺ>
    {
        private _ʺx28ʺTranscriber()
        {
        }

        public static _ʺx28ʺTranscriber Instance { get; } = new _ʺx28ʺTranscriber();

        public void Transcribe(Inners._ʺx28ʺ value, StringBuilder builder)
        {
            _x28Transcriber.Instance.Transcribe(value._x28_1, builder);
        }
    }
}
