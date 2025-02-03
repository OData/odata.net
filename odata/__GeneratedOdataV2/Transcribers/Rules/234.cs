namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _complexTypeNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._complexTypeName>
    {
        private _complexTypeNameTranscriber()
        {
        }
        
        public static _complexTypeNameTranscriber Instance { get; } = new _complexTypeNameTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._complexTypeName value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
