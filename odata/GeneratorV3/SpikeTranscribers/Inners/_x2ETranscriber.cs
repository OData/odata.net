namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _x2ETranscriber : ITranscriber<Inners._x2E>
    {
        private _x2ETranscriber()
        {
        }

        public static _x2ETranscriber Instance { get; } = new _x2ETranscriber();

        public void Transcribe(Inners._x2E value, StringBuilder builder)
        {
            builder.Append((char)0x2E);
        }
    }
}
