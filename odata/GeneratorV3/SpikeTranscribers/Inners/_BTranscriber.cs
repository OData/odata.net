namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _BTranscriber : ITranscriber<Inners._B>
    {
        private _BTranscriber()
        {
        }

        public static _BTranscriber Instance { get; } = new _BTranscriber();

        public void Transcribe(Inners._B value, StringBuilder builder)
        {
            builder.Append("B");
        }
    }
}
