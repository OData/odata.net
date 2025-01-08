namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _x25Transcriber : ITranscriber<Inners._x25>
    {
        private _x25Transcriber()
        {
        }

        public static _x25Transcriber Instance { get; } = new _x25Transcriber();

        public void Transcribe(Inners._x25 value, StringBuilder builder)
        {
            builder.Append((char)0x25);
        }
    }
}
