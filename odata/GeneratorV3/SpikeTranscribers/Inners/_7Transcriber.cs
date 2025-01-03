namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _7Transcriber : ITranscriber<Inners._7>
    {
        private _7Transcriber()
        {
        }

        public static _7Transcriber Instance { get; } = new _7Transcriber();

        public void Transcribe(Inners._7 value, StringBuilder builder)
        {
            builder.Append("7");
        }
    }
}
