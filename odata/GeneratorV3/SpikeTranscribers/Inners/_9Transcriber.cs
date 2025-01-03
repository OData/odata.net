namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _9Transcriber : ITranscriber<Inners._9>
    {
        private _9Transcriber()
        {
        }

        public static _9Transcriber Instance { get; } = new _9Transcriber();

        public void Transcribe(Inners._9 value, StringBuilder builder)
        {
            builder.Append("9");
        }
    }
}
