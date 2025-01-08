namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;
    using GeneratorV3.SpikeTranscribers.Rules;

    public sealed class _Жcⲻwsp_cⲻnlTranscriber : ITranscriber<Inners._Жcⲻwsp_cⲻnl>
    {
        private _Жcⲻwsp_cⲻnlTranscriber()
        {
        }

        public static _Жcⲻwsp_cⲻnlTranscriber Instance { get; } = new _Жcⲻwsp_cⲻnlTranscriber();

        public void Transcribe(Inners._Жcⲻwsp_cⲻnl value, StringBuilder builder)
        {
            foreach (var _cⲻwsp in value._cⲻwsp_1)
            {
                _cⲻwspTranscriber.Instance.Transcribe(_cⲻwsp, builder);
            }

            CnlTranscriber.Instance.Transcribe(value._cⲻnl_1, builder);
        }
    }
}
