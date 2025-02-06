namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _complexTypeNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._complexTypeName>
    {
        private _complexTypeNameTranscriber()
        {
        }
        
        public static _complexTypeNameTranscriber Instance { get; } = new _complexTypeNameTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._complexTypeName value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
