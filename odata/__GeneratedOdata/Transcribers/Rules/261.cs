namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _complexColFunctionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._complexColFunction>
    {
        private _complexColFunctionTranscriber()
        {
        }
        
        public static _complexColFunctionTranscriber Instance { get; } = new _complexColFunctionTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._complexColFunction value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
