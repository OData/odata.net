namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _FTranscriber : ITranscriber<Inners._F>
    {
        private _FTranscriber()
        {
        }

        public static _FTranscriber Instance { get; } = new _FTranscriber();

        public void Transcribe(Inners._F value, StringBuilder builder)
        {
            builder.Append("F");
        }
    }
}
