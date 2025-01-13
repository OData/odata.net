namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _primitiveColFunctionImportTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._primitiveColFunctionImport>
    {
        private _primitiveColFunctionImportTranscriber()
        {
        }
        
        public static _primitiveColFunctionImportTranscriber Instance { get; } = new _primitiveColFunctionImportTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._primitiveColFunctionImport value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
