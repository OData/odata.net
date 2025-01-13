namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _enumTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._enum>
    {
        private _enumTranscriber()
        {
        }
        
        public static _enumTranscriber Instance { get; } = new _enumTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._enum value, System.Text.StringBuilder builder)
        {
            if (value._qualifiedEnumTypeName_1 != null)
{
__GeneratedOdata.Trancsribers.Rules._qualifiedEnumTypeNameTranscriber.Instance.Transcribe(value._qualifiedEnumTypeName_1, builder);
}
__GeneratedOdata.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_1, builder);
__GeneratedOdata.Trancsribers.Rules._enumValueTranscriber.Instance.Transcribe(value._enumValue_1, builder);
__GeneratedOdata.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_2, builder);

        }
    }
    
}
