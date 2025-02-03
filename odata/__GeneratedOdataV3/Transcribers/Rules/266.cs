namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _complexFunctionImportTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._complexFunctionImport>
    {
        private _complexFunctionImportTranscriber()
        {
        }
        
        public static _complexFunctionImportTranscriber Instance { get; } = new _complexFunctionImportTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._complexFunctionImport value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
