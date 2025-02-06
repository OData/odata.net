namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _complexColFunctionImportTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._complexColFunctionImport>
    {
        private _complexColFunctionImportTranscriber()
        {
        }
        
        public static _complexColFunctionImportTranscriber Instance { get; } = new _complexColFunctionImportTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._complexColFunctionImport value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
