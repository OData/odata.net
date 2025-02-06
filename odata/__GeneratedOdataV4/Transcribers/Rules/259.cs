namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _entityColFunctionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._entityColFunction>
    {
        private _entityColFunctionTranscriber()
        {
        }
        
        public static _entityColFunctionTranscriber Instance { get; } = new _entityColFunctionTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._entityColFunction value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
