namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _annotationQualifierTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._annotationQualifier>
    {
        private _annotationQualifierTranscriber()
        {
        }
        
        public static _annotationQualifierTranscriber Instance { get; } = new _annotationQualifierTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._annotationQualifier value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
