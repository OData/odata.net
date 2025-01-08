namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _ʺx78ʺTranscriber : ITranscriber<Inners._ʺx78ʺ>
    {
        private _ʺx78ʺTranscriber()
        {
        }

        public static _ʺx78ʺTranscriber Instance { get; } = new _ʺx78ʺTranscriber();

        public void Transcribe(Inners._ʺx78ʺ value, StringBuilder builder)
        {
            _x78Transcriber.Instance.Transcribe(value._x78_1, builder);
        }
    }
}
