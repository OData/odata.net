namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _x2ATranscriber : ITranscriber<Inners._x2A>
    {
        private _x2ATranscriber()
        {
        }

        public static _x2ATranscriber Instance { get; } = new _x2ATranscriber();

        public void Transcribe(Inners._x2A value, StringBuilder builder)
        {
            builder.Append((char)0x2A);
        }
    }
}
