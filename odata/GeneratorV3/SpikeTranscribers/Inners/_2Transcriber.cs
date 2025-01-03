namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _2Transcriber : ITranscriber<Inners._2>
    {
        private _2Transcriber()
        {
        }

        public static _2Transcriber Instance { get; } = new _2Transcriber();

        public void Transcribe(Inners._2 value, StringBuilder builder)
        {
            builder.Append("2");
        }
    }
}
