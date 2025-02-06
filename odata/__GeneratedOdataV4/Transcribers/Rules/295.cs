namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _timeOfDayValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._timeOfDayValue>
    {
        private _timeOfDayValueTranscriber()
        {
        }
        
        public static _timeOfDayValueTranscriber Instance { get; } = new _timeOfDayValueTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._timeOfDayValue value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._hourTranscriber.Instance.Transcribe(value._hour_1, builder);
__GeneratedOdataV4.Trancsribers.Inners._ʺx3AʺTranscriber.Instance.Transcribe(value._ʺx3Aʺ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._minuteTranscriber.Instance.Transcribe(value._minute_1, builder);
if (value._ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡_1 != null)
{
__GeneratedOdataV4.Trancsribers.Inners._ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡Transcriber.Instance.Transcribe(value._ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡_1, builder);
}

        }
    }
    
}
