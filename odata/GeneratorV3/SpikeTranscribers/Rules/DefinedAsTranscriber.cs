namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;
    using GeneratorV3.SpikeTranscribers.Inners;

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
                _cⲻwspTranscriber.Instance.Transcribe(cⲻwsp, builder);
            }

            _Ⲥʺx3DʺⳆʺx3Dx2FʺↃTranscriber.Instance.Transcribe(value._Ⲥʺx3DʺⳆʺx3Dx2FʺↃ_1, builder);

            foreach (var cⲻwsp in value._cⲻwsp_2)
            {
                _cⲻwspTranscriber.Instance.Transcribe(cⲻwsp, builder);
            }
        }
    }
}
