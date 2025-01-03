namespace GeneratorV3.SpikeTranscribers.Inners
{
    using GeneratorV3.Abnf;
    using System.Text;

    public sealed class _ⲤWSPⳆCRLF_WSPↃTranscriber : ITranscriber<Inners._ⲤWSPⳆCRLF_WSPↃ>
    {
        private _ⲤWSPⳆCRLF_WSPↃTranscriber()
        {
        }

        public static _ⲤWSPⳆCRLF_WSPↃTranscriber Instance { get; } = new _ⲤWSPⳆCRLF_WSPↃTranscriber();

        public void Transcribe(Inners._ⲤWSPⳆCRLF_WSPↃ value, StringBuilder builder)
        {
            _WSPⳆCRLF_WSPTranscriber.Instance.Transcribe(value._WSPⳆCRLF_WSP_1, builder);
        }
    }
}
