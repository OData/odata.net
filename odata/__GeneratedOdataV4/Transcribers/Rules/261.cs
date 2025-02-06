namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _complexColFunctionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._complexColFunction>
    {
        private _complexColFunctionTranscriber()
        {
        }
        
        public static _complexColFunctionTranscriber Instance { get; } = new _complexColFunctionTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._complexColFunction value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
