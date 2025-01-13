namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _primitiveFunctionImportTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._primitiveFunctionImport>
    {
        private _primitiveFunctionImportTranscriber()
        {
        }
        
        public static _primitiveFunctionImportTranscriber Instance { get; } = new _primitiveFunctionImportTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._primitiveFunctionImport value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
