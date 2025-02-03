namespace __GeneratedOdataV2.Trancsribers.Inners
{
    public sealed class _COMMA_singleEnumValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Inners._COMMA_singleEnumValue>
    {
        private _COMMA_singleEnumValueTranscriber()
        {
        }
        
        public static _COMMA_singleEnumValueTranscriber Instance { get; } = new _COMMA_singleEnumValueTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Inners._COMMA_singleEnumValue value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._singleEnumValueTranscriber.Instance.Transcribe(value._singleEnumValue_1, builder);

        }
    }
    
}
