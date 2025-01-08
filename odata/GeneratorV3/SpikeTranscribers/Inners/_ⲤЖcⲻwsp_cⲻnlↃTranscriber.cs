namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _ⲤЖcⲻwsp_cⲻnlↃTranscriber : ITranscriber<Inners._ⲤЖcⲻwsp_cⲻnlↃ>
    {
        private _ⲤЖcⲻwsp_cⲻnlↃTranscriber()
        {
        }

        public static _ⲤЖcⲻwsp_cⲻnlↃTranscriber Instance { get; } = new _ⲤЖcⲻwsp_cⲻnlↃTranscriber();

        public void Transcribe(Inners._ⲤЖcⲻwsp_cⲻnlↃ value, StringBuilder builder)
        {
            _Жcⲻwsp_cⲻnlTranscriber.Instance.Transcribe(value._Жcⲻwsp_cⲻnl_1, builder);
        }
    }
}
