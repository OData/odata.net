namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _primitiveColFunctionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._primitiveColFunction>
    {
        private _primitiveColFunctionTranscriber()
        {
        }
        
        public static _primitiveColFunctionTranscriber Instance { get; } = new _primitiveColFunctionTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._primitiveColFunction value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
