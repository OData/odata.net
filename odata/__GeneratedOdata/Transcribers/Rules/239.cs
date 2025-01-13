namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _odataIdentifierTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._odataIdentifier>
    {
        private _odataIdentifierTranscriber()
        {
        }
        
        public static _odataIdentifierTranscriber Instance { get; } = new _odataIdentifierTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._odataIdentifier value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._identifierLeadingCharacterTranscriber.Instance.Transcribe(value._identifierLeadingCharacter_1, builder);
foreach (var _identifierCharacter_1 in value._identifierCharacter_1)
{
__GeneratedOdata.Trancsribers.Rules._identifierCharacterTranscriber.Instance.Transcribe(_identifierCharacter_1, builder);
}

        }
    }
    
}
