namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _x78Transcriber : ITranscriber<Inners._x78>
    {
        private _x78Transcriber()
        {
        }

        public static _x78Transcriber Instance { get; } = new _x78Transcriber();

        public void Transcribe(Inners._x78 value, StringBuilder builder)
        {
            builder.Append((char)0x78);
        }
    }
}
