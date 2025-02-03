namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _parameterNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._parameterName>
    {
        private _parameterNameTranscriber()
        {
        }
        
        public static _parameterNameTranscriber Instance { get; } = new _parameterNameTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._parameterName value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
