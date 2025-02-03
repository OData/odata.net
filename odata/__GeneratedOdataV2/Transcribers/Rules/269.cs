namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _primitiveColFunctionImportTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._primitiveColFunctionImport>
    {
        private _primitiveColFunctionImportTranscriber()
        {
        }
        
        public static _primitiveColFunctionImportTranscriber Instance { get; } = new _primitiveColFunctionImportTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._primitiveColFunctionImport value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
