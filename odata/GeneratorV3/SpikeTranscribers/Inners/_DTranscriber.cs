namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _DTranscriber : ITranscriber<Inners._D>
    {
        private _DTranscriber()
        {
        }

        public static _DTranscriber Instance { get; } = new _DTranscriber();

        public void Transcribe(Inners._D value, StringBuilder builder)
        {
            builder.Append("D");
        }
    }
}
