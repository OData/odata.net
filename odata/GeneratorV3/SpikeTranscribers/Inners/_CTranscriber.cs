namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _CTranscriber : ITranscriber<Inners._C>
    {
        private _CTranscriber()
        {
        }

        public static _CTranscriber Instance { get; } = new _CTranscriber();

        public void Transcribe(Inners._C value, StringBuilder builder)
        {
            builder.Append("C");
        }
    }
}
