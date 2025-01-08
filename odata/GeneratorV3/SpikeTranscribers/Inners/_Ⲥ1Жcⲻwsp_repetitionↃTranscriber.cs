namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _Ⲥ1Жcⲻwsp_repetitionↃTranscriber : ITranscriber<Inners._Ⲥ1Жcⲻwsp_repetitionↃ>
    {
        private _Ⲥ1Жcⲻwsp_repetitionↃTranscriber()
        {
        }

        public static _Ⲥ1Жcⲻwsp_repetitionↃTranscriber Instance { get; } = new _Ⲥ1Жcⲻwsp_repetitionↃTranscriber();

        public void Transcribe(Inners._Ⲥ1Жcⲻwsp_repetitionↃ value, StringBuilder builder)
        {
            _1Жcⲻwsp_repetitionTranscriber.Instance.Transcribe(value._1Жcⲻwsp_repetition_1, builder);
        }
    }
}
