namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _ʺx3Dx2FʺTranscriber : ITranscriber<Inners._ʺx3Dx2Fʺ>
    {
        private _ʺx3Dx2FʺTranscriber()
        {
        }

        public static _ʺx3Dx2FʺTranscriber Instance { get; } = new _ʺx3Dx2FʺTranscriber();

        public void Transcribe(Inners._ʺx3Dx2Fʺ value, StringBuilder builder)
        {
            _x3DTranscriber.Instance.Transcribe(value._x3D_1, builder);
            _x2FTranscriber.Instance.Transcribe(value._x2F_1, builder);
        }
    }
}
