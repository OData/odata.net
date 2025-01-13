namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _timeOfDayValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._timeOfDayValue>
    {
        private _timeOfDayValueTranscriber()
        {
        }
        
        public static _timeOfDayValueTranscriber Instance { get; } = new _timeOfDayValueTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._timeOfDayValue value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._hourTranscriber.Instance.Transcribe(value._hour_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx3AʺTranscriber.Instance.Transcribe(value._ʺx3Aʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._minuteTranscriber.Instance.Transcribe(value._minute_1, builder);
if (value._ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡Transcriber.Instance.Transcribe(value._ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡_1, builder);
}

        }
    }
    
}
