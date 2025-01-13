namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _complexFunctionImportTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._complexFunctionImport>
    {
        private _complexFunctionImportTranscriber()
        {
        }
        
        public static _complexFunctionImportTranscriber Instance { get; } = new _complexFunctionImportTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._complexFunctionImport value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
