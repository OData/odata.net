namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _actionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._action>
    {
        private _actionTranscriber()
        {
        }
        
        public static _actionTranscriber Instance { get; } = new _actionTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._action value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
