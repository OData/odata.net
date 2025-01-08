namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;
    using GeneratorV3.SpikeTranscribers.Rules;

    public sealed class _cⲻnl_WSPTranscriber : ITranscriber<Inners._cⲻnl_WSP>
    {
        private _cⲻnl_WSPTranscriber()
        {
        }

        public static _cⲻnl_WSPTranscriber Instance { get; } = new _cⲻnl_WSPTranscriber();

        public void Transcribe(Inners._cⲻnl_WSP value, StringBuilder builder)
        {
            CnlTranscriber.Instance.Transcribe(value._cⲻnl_1, builder);
            WspTranscriber.Instance.Transcribe(value._WSP_1, builder);
        }
    }
}
