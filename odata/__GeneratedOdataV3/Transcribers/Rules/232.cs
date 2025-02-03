namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _singletonEntityTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._singletonEntity>
    {
        private _singletonEntityTranscriber()
        {
        }
        
        public static _singletonEntityTranscriber Instance { get; } = new _singletonEntityTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._singletonEntity value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
