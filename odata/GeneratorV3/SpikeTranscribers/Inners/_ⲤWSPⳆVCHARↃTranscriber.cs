namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _ⲤWSPⳆVCHARↃTranscriber : ITranscriber<Inners._ⲤWSPⳆVCHARↃ>
    {
        private _ⲤWSPⳆVCHARↃTranscriber()
        {
        }

        public static _ⲤWSPⳆVCHARↃTranscriber Instance { get; } = new _ⲤWSPⳆVCHARↃTranscriber();

        public void Transcribe(Inners._ⲤWSPⳆVCHARↃ value, StringBuilder builder)
        {
            _WSPⳆVCHARTranscriber.Instance.Transcribe(value._WSPⳆVCHAR_1, builder);
        }
    }
}
