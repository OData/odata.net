namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _entityColFunctionImportTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._entityColFunctionImport>
    {
        private _entityColFunctionImportTranscriber()
        {
        }
        
        public static _entityColFunctionImportTranscriber Instance { get; } = new _entityColFunctionImportTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._entityColFunctionImport value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
