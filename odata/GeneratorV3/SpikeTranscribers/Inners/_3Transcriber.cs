namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _3Transcriber : ITranscriber<Inners._3>
    {
        private _3Transcriber()
        {
        }

        public static _3Transcriber Instance { get; } = new _3Transcriber();

        public void Transcribe(Inners._3 value, StringBuilder builder)
        {
            builder.Append("3");
        }
    }
}
