namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _ʺx5BʺTranscriber : ITranscriber<Inners._ʺx5Bʺ>
    {
        private _ʺx5BʺTranscriber()
        {
        }

        public static _ʺx5BʺTranscriber Instance { get; } = new _ʺx5BʺTranscriber();

        public void Transcribe(Inners._ʺx5Bʺ value, StringBuilder builder)
        {
            _x5BTranscriber.Instance.Transcribe(value._x5B_1, builder);
        }
    }
}
