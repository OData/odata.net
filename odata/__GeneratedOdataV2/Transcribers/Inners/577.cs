namespace __GeneratedOdataV2.Trancsribers.Inners
{
    public sealed class _EQⲻh_booleanValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Inners._EQⲻh_booleanValue>
    {
        private _EQⲻh_booleanValueTranscriber()
        {
        }
        
        public static _EQⲻh_booleanValueTranscriber Instance { get; } = new _EQⲻh_booleanValueTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Inners._EQⲻh_booleanValue value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._EQⲻhTranscriber.Instance.Transcribe(value._EQⲻh_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._booleanValueTranscriber.Instance.Transcribe(value._booleanValue_1, builder);

        }
    }
    
}
