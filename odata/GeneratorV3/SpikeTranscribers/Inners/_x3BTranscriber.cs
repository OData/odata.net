namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _x3BTranscriber : ITranscriber<Inners._x3B>
    {
        private _x3BTranscriber()
        {
        }

        public static _x3BTranscriber Instance { get; } = new _x3BTranscriber();

        public void Transcribe(Inners._x3B value, StringBuilder builder)
        {
            builder.Append((char)0x3B);
        }
    }
}
