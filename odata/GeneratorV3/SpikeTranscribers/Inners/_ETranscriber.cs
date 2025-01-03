namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _ETranscriber : ITranscriber<Inners._E>
    {
        private _ETranscriber()
        {
        }

        public static _ETranscriber Instance { get; } = new _ETranscriber();

        public void Transcribe(Inners._E value, StringBuilder builder)
        {
            builder.Append("E");
        }
    }
}
