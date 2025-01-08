namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class DefinedAsTranscriber : ITranscriber<_definedⲻas>
    {
        private DefinedAsTranscriber()
        {
        }

        public static DefinedAsTranscriber Instance { get; } = new DefinedAsTranscriber();

        public void Transcribe(_definedⲻas value, StringBuilder builder)
        {
            foreach (var cⲻwsp in value._cⲻwsp_1)
            {
                _cⲻwspTranscriber.Instance.Transcribe(cⲻwsp)
            }


        }
    }
}
