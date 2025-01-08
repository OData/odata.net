namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _elementsTranscriber : ITranscriber<_elements>
    {
        private _elementsTranscriber()
        {
        }

        public static _elementsTranscriber Instance { get; } = new _elementsTranscriber();

        public void Transcribe(_elements value, StringBuilder builder)
        {
        }
    }
}
