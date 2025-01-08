namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _x5DTranscriber : ITranscriber<Inners._x5D>
    {
        private _x5DTranscriber()
        {
        }

        public static _x5DTranscriber Instance { get; } = new _x5DTranscriber();

        public void Transcribe(Inners._x5D value, StringBuilder builder)
        {
            builder.Append((char)0x5D);
        }
    }
}
