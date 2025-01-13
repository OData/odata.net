namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _actionImportTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._actionImport>
    {
        private _actionImportTranscriber()
        {
        }
        
        public static _actionImportTranscriber Instance { get; } = new _actionImportTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._actionImport value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
