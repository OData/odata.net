namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _ⲤALPHAⳆDIGITⳆʺx2DʺↃTranscriber : ITranscriber<Inners._ⲤALPHAⳆDIGITⳆʺx2DʺↃ>
    {
        private _ⲤALPHAⳆDIGITⳆʺx2DʺↃTranscriber()
        {
        }

        public static _ⲤALPHAⳆDIGITⳆʺx2DʺↃTranscriber Instance { get; } = new _ⲤALPHAⳆDIGITⳆʺx2DʺↃTranscriber();

        public void Transcribe(Inners._ⲤALPHAⳆDIGITⳆʺx2DʺↃ value, StringBuilder builder)
        {
            _ALPHAⳆDIGITⳆʺx2DʺTranscriber.Instance.Transcribe(value._ALPHAⳆDIGITⳆʺx2Dʺ_1, builder);
        }
    }
}
