namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _x2FTranscriber : ITranscriber<Inners._x2F>
    {
        private _x2FTranscriber()
        {
        }

        public static _x2FTranscriber Instance { get; } = new _x2FTranscriber();

        public void Transcribe(Inners._x2F value, StringBuilder builder)
        {
            builder.Append((char)0x2F);
        }
    }
}
