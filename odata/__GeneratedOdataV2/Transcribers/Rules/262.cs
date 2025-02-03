namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _primitiveFunctionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._primitiveFunction>
    {
        private _primitiveFunctionTranscriber()
        {
        }
        
        public static _primitiveFunctionTranscriber Instance { get; } = new _primitiveFunctionTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._primitiveFunction value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
