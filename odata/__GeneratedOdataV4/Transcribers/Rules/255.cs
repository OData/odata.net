namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _actionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._action>
    {
        private _actionTranscriber()
        {
        }
        
        public static _actionTranscriber Instance { get; } = new _actionTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._action value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
