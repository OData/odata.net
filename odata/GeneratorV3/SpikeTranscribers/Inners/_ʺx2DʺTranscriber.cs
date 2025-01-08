namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _ʺx2DʺTranscriber : ITranscriber<Inners._ʺx2Dʺ>
    {
        private _ʺx2DʺTranscriber()
        {
        }

        public static _ʺx2DʺTranscriber Instance { get; } = new _ʺx2DʺTranscriber();

        public void Transcribe(Inners._ʺx2Dʺ value, StringBuilder builder)
        {
            _x2DTranscriber.Instance.Transcribe(value._x2D_1, builder);
        }
    }
}
