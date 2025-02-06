namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _enumerationMemberTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._enumerationMember>
    {
        private _enumerationMemberTranscriber()
        {
        }
        
        public static _enumerationMemberTranscriber Instance { get; } = new _enumerationMemberTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._enumerationMember value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
