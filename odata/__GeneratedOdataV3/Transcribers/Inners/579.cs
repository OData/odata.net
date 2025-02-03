namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _COMMA_annotationIdentifierTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._COMMA_annotationIdentifier>
    {
        private _COMMA_annotationIdentifierTranscriber()
        {
        }
        
        public static _COMMA_annotationIdentifierTranscriber Instance { get; } = new _COMMA_annotationIdentifierTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._COMMA_annotationIdentifier value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._annotationIdentifierTranscriber.Instance.Transcribe(value._annotationIdentifier_1, builder);

        }
    }
    
}
