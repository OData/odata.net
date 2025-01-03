namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _8Transcriber : ITranscriber<Inners._8>
    {
        private _8Transcriber()
        {
        }

        public static _8Transcriber Instance { get; } = new _8Transcriber();

        public void Transcribe(Inners._8 value, StringBuilder builder)
        {
            builder.Append("8");
        }
    }
}
