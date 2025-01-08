namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;
    using GeneratorV3.SpikeTranscribers.Inners;

    public sealed class _hexⲻvalTranscriber : ITranscriber<_hexⲻval>
    {
        private _hexⲻvalTranscriber()
        {
        }

        public static _hexⲻvalTranscriber Instance { get; } = new _hexⲻvalTranscriber();

        public void Transcribe(_hexⲻval value, StringBuilder builder)
        {
            _ʺx78ʺTranscriber.Instance.Transcribe(value._ʺx78ʺ_1, builder);
            foreach (var _HEXDIG in value._HEXDIG_1)
            {
                HexDigTranscriber.Instance.Transcribe(_HEXDIG, builder);
            }

            if (value._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ_1 != null)
            {
                _1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃTranscriber.Instance.Transcribe(value._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ_1, builder);
            }
        }
    }
}
