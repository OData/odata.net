namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _dateValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._dateValue>
    {
        private _dateValueTranscriber()
        {
        }
        
        public static _dateValueTranscriber Instance { get; } = new _dateValueTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._dateValue value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._yearTranscriber.Instance.Transcribe(value._year_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx2DʺTranscriber.Instance.Transcribe(value._ʺx2Dʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._monthTranscriber.Instance.Transcribe(value._month_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx2DʺTranscriber.Instance.Transcribe(value._ʺx2Dʺ_2, builder);
__GeneratedOdata.Trancsribers.Rules._dayTranscriber.Instance.Transcribe(value._day_1, builder);

        }
    }
    
}
