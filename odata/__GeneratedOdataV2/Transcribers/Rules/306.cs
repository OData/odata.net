namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _enumValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._enumValue>
    {
        private _enumValueTranscriber()
        {
        }
        
        public static _enumValueTranscriber Instance { get; } = new _enumValueTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._enumValue value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._singleEnumValueTranscriber.Instance.Transcribe(value._singleEnumValue_1, builder);
foreach (var _ⲤCOMMA_singleEnumValueↃ_1 in value._ⲤCOMMA_singleEnumValueↃ_1)
{
Inners._ⲤCOMMA_singleEnumValueↃTranscriber.Instance.Transcribe(_ⲤCOMMA_singleEnumValueↃ_1, builder);
}

        }
    }
    
}
