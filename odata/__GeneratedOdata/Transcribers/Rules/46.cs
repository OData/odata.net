namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _parameterNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._parameterName>
    {
        private _parameterNameTranscriber()
        {
        }
        
        public static _parameterNameTranscriber Instance { get; } = new _parameterNameTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._parameterName value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
