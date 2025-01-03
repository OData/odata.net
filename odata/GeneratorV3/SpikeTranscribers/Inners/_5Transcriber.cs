namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _5Transcriber : ITranscriber<Inners._5>
    {
        private _5Transcriber()
        {
        }

        public static _5Transcriber Instance { get; } = new _5Transcriber();

        public void Transcribe(Inners._5 value, StringBuilder builder)
        {
            builder.Append("5");
        }
    }
}
