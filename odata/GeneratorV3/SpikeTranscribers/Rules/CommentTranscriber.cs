namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;
    using GeneratorV3.SpikeTranscribers.Inners;

    public sealed class CommentTranscriber : ITranscriber<_comment>
    {
        private CommentTranscriber()
        {
        }

        public static CommentTranscriber Instance { get; } = new CommentTranscriber();

        public void Transcribe(_comment value, StringBuilder builder)
        {
            _ʺx3BʺTranscriber.Instance.Transcribe(value._ʺx3Bʺ_1, builder);
            foreach (var _ⲤWSPⳆVCHARↃ in value._ⲤWSPⳆVCHARↃ_1)
            {
                _ⲤWSPⳆVCHARↃTranscriber.Instance.Transcribe(_ⲤWSPⳆVCHARↃ, builder);
            }

            CrLfTranscriber.Instance.Transcribe(value._CRLF_1, builder);
        }
    }
}
