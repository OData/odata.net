namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class CharTranscriber : ITranscriber<_CHAR>
    {
        private CharTranscriber()
        {
        }

        public static CharTranscriber Instance { get; } = new CharTranscriber();

        public void Transcribe(_CHAR value, StringBuilder builder)
        {
            
        }
    }
}
