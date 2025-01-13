namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _dateTimeOffsetValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._dateTimeOffsetValue>
    {
        private _dateTimeOffsetValueTranscriber()
        {
        }
        
        public static _dateTimeOffsetValueTranscriber Instance { get; } = new _dateTimeOffsetValueTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._dateTimeOffsetValue value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._yearTranscriber.Instance.Transcribe(value._year_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx2DʺTranscriber.Instance.Transcribe(value._ʺx2Dʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._monthTranscriber.Instance.Transcribe(value._month_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx2DʺTranscriber.Instance.Transcribe(value._ʺx2Dʺ_2, builder);
__GeneratedOdata.Trancsribers.Rules._dayTranscriber.Instance.Transcribe(value._day_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx54ʺTranscriber.Instance.Transcribe(value._ʺx54ʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._hourTranscriber.Instance.Transcribe(value._hour_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx3AʺTranscriber.Instance.Transcribe(value._ʺx3Aʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._minuteTranscriber.Instance.Transcribe(value._minute_1, builder);
if (value._ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡Transcriber.Instance.Transcribe(value._ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡_1, builder);
}
__GeneratedOdata.Trancsribers.Inners._Ⲥʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minuteↃTranscriber.Instance.Transcribe(value._Ⲥʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minuteↃ_1, builder);

        }
    }
    
}
