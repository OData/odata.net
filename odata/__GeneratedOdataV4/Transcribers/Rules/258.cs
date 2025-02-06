namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _entityFunctionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._entityFunction>
    {
        private _entityFunctionTranscriber()
        {
        }
        
        public static _entityFunctionTranscriber Instance { get; } = new _entityFunctionTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._entityFunction value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
