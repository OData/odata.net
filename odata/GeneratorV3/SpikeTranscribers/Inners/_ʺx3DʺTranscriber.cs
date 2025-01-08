namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _ʺx3DʺTranscriber : ITranscriber<Inners._ʺx3Dʺ>
    {
        private _ʺx3DʺTranscriber()
        {
        }

        public static _ʺx3DʺTranscriber Instance { get; } = new _ʺx3DʺTranscriber();

        public void Transcribe(Inners._ʺx3Dʺ value, StringBuilder builder)
        {
            _x3DTranscriber.Instance.Transcribe(value._x3D_1, builder);
        }
    }
}
