namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _ʺx5DʺTranscriber : ITranscriber<Inners._ʺx5Dʺ>
    {
        private _ʺx5DʺTranscriber()
        {
        }

        public static _ʺx5DʺTranscriber Instance { get; } = new _ʺx5DʺTranscriber();

        public void Transcribe(Inners._ʺx5Dʺ value, StringBuilder builder)
        {
            _x5DTranscriber.Instance.Transcribe(value._x5D_1, builder);
        }
    }
}
