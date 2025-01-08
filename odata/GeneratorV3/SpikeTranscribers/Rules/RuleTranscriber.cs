namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class RuleTranscriber : ITranscriber<_rule>
    {
        private RuleTranscriber()
        {
        }
        
        public static RuleTranscriber Instance { get; } = new RuleTranscriber();

        public void Transcribe(_rule value, StringBuilder builder)
        {
            RuleNameTranscriber.Instance.Transcribe(value._rulename_1, builder);
            DefinedAsTranscriber.Instance.Transcribe(value._definedⲻas_1, builder);
            _elementsTranscriber.Instance.Transcribe(value._elements_1, builder);
            CnlTranscriber.Instance.Transcribe(value._cⲻnl_1, builder);
        }
    }
}
