namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _entityColFunctionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._entityColFunction>
    {
        private _entityColFunctionTranscriber()
        {
        }
        
        public static _entityColFunctionTranscriber Instance { get; } = new _entityColFunctionTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._entityColFunction value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
