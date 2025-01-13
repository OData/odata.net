namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _entityTypeNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._entityTypeName>
    {
        private _entityTypeNameTranscriber()
        {
        }
        
        public static _entityTypeNameTranscriber Instance { get; } = new _entityTypeNameTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._entityTypeName value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
