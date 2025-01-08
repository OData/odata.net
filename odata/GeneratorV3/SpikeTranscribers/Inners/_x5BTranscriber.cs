namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _x5BTranscriber : ITranscriber<Inners._x5B>
    {
        private _x5BTranscriber()
        {
        }

        public static _x5BTranscriber Instance { get; } = new _x5BTranscriber();

        public void Transcribe(Inners._x5B value, StringBuilder builder)
        {
            builder.Append((char)0x5B);
        }
    }
}
