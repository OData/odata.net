namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _enumerationTypeNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._enumerationTypeName>
    {
        private _enumerationTypeNameTranscriber()
        {
        }
        
        public static _enumerationTypeNameTranscriber Instance { get; } = new _enumerationTypeNameTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._enumerationTypeName value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
