namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;
    using GeneratorV3.SpikeTranscribers.Inners;

    public sealed class LwspTranscriber : ITranscriber<_LWSP>
    {
        private LwspTranscriber()
        {
        }

        public static LwspTranscriber Instance { get; } = new LwspTranscriber();

        public void Transcribe(_LWSP value, StringBuilder builder)
        {
            foreach (var _ⲤWSPⳆCRLF_WSPↃ in value._ⲤWSPⳆCRLF_WSPↃ_1)
            {
                _ⲤWSPⳆCRLF_WSPↃTranscriber.Instance.Transcribe(_ⲤWSPⳆCRLF_WSPↃ, builder);
            }
        }
    }
}
