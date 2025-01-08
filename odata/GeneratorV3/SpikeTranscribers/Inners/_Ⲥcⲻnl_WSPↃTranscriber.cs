namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _Ⲥcⲻnl_WSPↃTranscriber : ITranscriber<Inners._Ⲥcⲻnl_WSPↃ>
    {
        private _Ⲥcⲻnl_WSPↃTranscriber()
        {
        }

        public static _Ⲥcⲻnl_WSPↃTranscriber Instance { get; } = new _Ⲥcⲻnl_WSPↃTranscriber();

        public void Transcribe(Inners._Ⲥcⲻnl_WSPↃ value, StringBuilder builder)
        {
            _cⲻnl_WSPTranscriber.Instance.Transcribe(value._cⲻnl_WSP_1, builder);
        }
    }
}
