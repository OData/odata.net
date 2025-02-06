namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _singletonEntityTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._singletonEntity>
    {
        private _singletonEntityTranscriber()
        {
        }
        
        public static _singletonEntityTranscriber Instance { get; } = new _singletonEntityTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._singletonEntity value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
