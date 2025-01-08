namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _x28Transcriber : ITranscriber<Inners._x28>
    {
        private _x28Transcriber()
        {
        }

        public static _x28Transcriber Instance { get; } = new _x28Transcriber();

        public void Transcribe(Inners._x28 value, StringBuilder builder)
        {
            builder.Append((char)0x28);
        }
    }
}
