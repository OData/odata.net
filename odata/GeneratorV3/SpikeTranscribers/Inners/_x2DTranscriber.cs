namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _x2DTranscriber : ITranscriber<Inners._x2D>
    {
        private _x2DTranscriber()
        {
        }

        public static _x2DTranscriber Instance { get; } = new _x2DTranscriber();

        public void Transcribe(Inners._x2D value, StringBuilder builder)
        {
            builder.Append((char)0x2D);
        }
    }
}
