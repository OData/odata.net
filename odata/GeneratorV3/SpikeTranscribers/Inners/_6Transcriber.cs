namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _6Transcriber : ITranscriber<Inners._6>
    {
        private _6Transcriber()
        {
        }

        public static _6Transcriber Instance { get; } = new _6Transcriber();

        public void Transcribe(Inners._6 value, StringBuilder builder)
        {
            builder.Append("6");
        }
    }
}
