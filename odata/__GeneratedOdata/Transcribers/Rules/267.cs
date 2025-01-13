namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _complexColFunctionImportTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._complexColFunctionImport>
    {
        private _complexColFunctionImportTranscriber()
        {
        }
        
        public static _complexColFunctionImportTranscriber Instance { get; } = new _complexColFunctionImportTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._complexColFunctionImport value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
