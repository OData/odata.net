namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃTranscriber : ITranscriber<Inners._ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ>
    {
        private _ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃTranscriber()
        {
        }

        public static _ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃTranscriber Instance { get; } = new _ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃTranscriber();

        public void Transcribe(Inners._ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ value, StringBuilder builder)
        {
            _binⲻvalⳆdecⲻvalⳆhexⲻvalTranscriber.Instance.Transcribe(value._binⲻvalⳆdecⲻvalⳆhexⲻval_1, builder);
        }
    }
}
