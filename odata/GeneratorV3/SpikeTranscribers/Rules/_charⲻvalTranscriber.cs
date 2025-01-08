namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;
    using GeneratorV3.SpikeTranscribers.Inners;

    public sealed class _charⲻvalTranscriber : ITranscriber<_charⲻval>
    {
        private _charⲻvalTranscriber()
        {
        }

        public static _charⲻvalTranscriber Instance { get; } = new _charⲻvalTranscriber();

        public void Transcribe(_charⲻval value, StringBuilder builder)
        {
            DquoteTranscriber.Instance.Transcribe(value._DQUOTE_1, builder);
            foreach (var _ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃ in value._ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃ_1)
            {
                _ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃTranscriber.Instance.Transcribe(_ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃ, builder);
            }

            DquoteTranscriber.Instance.Transcribe(value._DQUOTE_2, builder);
        }
    }
}
