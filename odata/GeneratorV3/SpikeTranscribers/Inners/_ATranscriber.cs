namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _ATranscriber : ITranscriber<Inners._A>
    {
        private _ATranscriber()
        {
        }

        public static _ATranscriber Instance { get; } = new _ATranscriber();

        public void Transcribe(Inners._A value, StringBuilder builder)
        {
            builder.Append("A");
        }
    }
}
