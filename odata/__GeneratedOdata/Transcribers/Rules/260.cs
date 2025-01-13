namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _complexFunctionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._complexFunction>
    {
        private _complexFunctionTranscriber()
        {
        }
        
        public static _complexFunctionTranscriber Instance { get; } = new _complexFunctionTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._complexFunction value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
