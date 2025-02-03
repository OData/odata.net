namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _typeDefinitionNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._typeDefinitionName>
    {
        private _typeDefinitionNameTranscriber()
        {
        }
        
        public static _typeDefinitionNameTranscriber Instance { get; } = new _typeDefinitionNameTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._typeDefinitionName value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
