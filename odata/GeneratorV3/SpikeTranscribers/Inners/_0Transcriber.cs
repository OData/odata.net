namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _0Transcriber : ITranscriber<Inners._0>
    {
        private _0Transcriber()
        {
        }

        public static _0Transcriber Instance { get; } = new _0Transcriber();

        public void Transcribe(Inners._0 value, StringBuilder builder)
        {
            builder.Append("0");
        }
    }
}
