namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _primitiveColFunctionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._primitiveColFunction>
    {
        private _primitiveColFunctionTranscriber()
        {
        }
        
        public static _primitiveColFunctionTranscriber Instance { get; } = new _primitiveColFunctionTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._primitiveColFunction value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
