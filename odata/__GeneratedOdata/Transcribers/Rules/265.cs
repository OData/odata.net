namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _entityColFunctionImportTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._entityColFunctionImport>
    {
        private _entityColFunctionImportTranscriber()
        {
        }
        
        public static _entityColFunctionImportTranscriber Instance { get; } = new _entityColFunctionImportTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._entityColFunctionImport value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
