namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _enumValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._enumValue>
    {
        private _enumValueTranscriber()
        {
        }
        
        public static _enumValueTranscriber Instance { get; } = new _enumValueTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._enumValue value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._singleEnumValueTranscriber.Instance.Transcribe(value._singleEnumValue_1, builder);
foreach (var _ⲤCOMMA_singleEnumValueↃ_1 in value._ⲤCOMMA_singleEnumValueↃ_1)
{
__GeneratedOdata.Trancsribers.Inners._ⲤCOMMA_singleEnumValueↃTranscriber.Instance.Transcribe(_ⲤCOMMA_singleEnumValueↃ_1, builder);
}

        }
    }
    
}