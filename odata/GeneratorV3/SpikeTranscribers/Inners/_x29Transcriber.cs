namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _x29Transcriber : ITranscriber<Inners._x29>
    {
        private _x29Transcriber()
        {
        }

        public static _x29Transcriber Instance { get; } = new _x29Transcriber();

        public void Transcribe(Inners._x29 value, StringBuilder builder)
        {
            builder.Append((char)0x29);
        }
    }
}
