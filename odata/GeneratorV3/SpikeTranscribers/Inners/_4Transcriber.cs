namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _4Transcriber : ITranscriber<Inners._4>
    {
        private _4Transcriber()
        {
        }

        public static _4Transcriber Instance { get; } = new _4Transcriber();

        public void Transcribe(Inners._4 value, StringBuilder builder)
        {
            builder.Append("4");
        }
    }
}
