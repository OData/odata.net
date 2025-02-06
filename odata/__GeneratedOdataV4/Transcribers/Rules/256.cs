namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _actionImportTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._actionImport>
    {
        private _actionImportTranscriber()
        {
        }
        
        public static _actionImportTranscriber Instance { get; } = new _actionImportTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._actionImport value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
