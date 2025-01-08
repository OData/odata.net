namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _x3DTranscriber : ITranscriber<Inners._x3D>
    {
        private _x3DTranscriber()
        {
        }

        public static _x3DTranscriber Instance { get; } = new _x3DTranscriber();

        public void Transcribe(Inners._x3D value, StringBuilder builder)
        {
            builder.Append((char)0x3D);
        }
    }
}
