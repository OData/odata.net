namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;
    using GeneratorV3.SpikeTranscribers.Inners;

    public sealed class RuleNameTranscriber : ITranscriber<_rulename>
    {
        private RuleNameTranscriber()
        {
        }

        public static RuleNameTranscriber Instance { get; } = new RuleNameTranscriber();

        public void Transcribe(_rulename value, StringBuilder builder)
        {
            AlphaTranscriber.Instance.Transcribe(value._ALPHA_1, builder);
            foreach (var ⲤALPHAⳆDIGITⳆʺx2DʺↃ in value._ⲤALPHAⳆDIGITⳆʺx2DʺↃ_1)
            {
                _ⲤALPHAⳆDIGITⳆʺx2DʺↃTranscriber.Instance.Transcribe(ⲤALPHAⳆDIGITⳆʺx2DʺↃ, builder);
            }
        }
    }
}
