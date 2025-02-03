namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _entityFunctionImportTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._entityFunctionImport>
    {
        private _entityFunctionImportTranscriber()
        {
        }
        
        public static _entityFunctionImportTranscriber Instance { get; } = new _entityFunctionImportTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._entityFunctionImport value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
