namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _1Transcriber : ITranscriber<Inners._1>
    {
        private _1Transcriber()
        {
        }

        public static _1Transcriber Instance { get; } = new _1Transcriber();

        public void Transcribe(Inners._1 value, StringBuilder builder)
        {
            builder.Append("1");
        }
    }
}
