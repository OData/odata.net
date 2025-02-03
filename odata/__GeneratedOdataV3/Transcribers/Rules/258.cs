namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _entityFunctionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._entityFunction>
    {
        private _entityFunctionTranscriber()
        {
        }
        
        public static _entityFunctionTranscriber Instance { get; } = new _entityFunctionTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._entityFunction value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
