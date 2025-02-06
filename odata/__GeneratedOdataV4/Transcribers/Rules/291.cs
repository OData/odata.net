namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _dateValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._dateValue>
    {
        private _dateValueTranscriber()
        {
        }
        
        public static _dateValueTranscriber Instance { get; } = new _dateValueTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._dateValue value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._yearTranscriber.Instance.Transcribe(value._year_1, builder);
__GeneratedOdataV4.Trancsribers.Inners._ʺx2DʺTranscriber.Instance.Transcribe(value._ʺx2Dʺ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._monthTranscriber.Instance.Transcribe(value._month_1, builder);
__GeneratedOdataV4.Trancsribers.Inners._ʺx2DʺTranscriber.Instance.Transcribe(value._ʺx2Dʺ_2, builder);
__GeneratedOdataV4.Trancsribers.Rules._dayTranscriber.Instance.Transcribe(value._day_1, builder);

        }
    }
    
}
