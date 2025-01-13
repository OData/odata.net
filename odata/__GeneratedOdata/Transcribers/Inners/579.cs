namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _COMMA_annotationIdentifierTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._COMMA_annotationIdentifier>
    {
        private _COMMA_annotationIdentifierTranscriber()
        {
        }
        
        public static _COMMA_annotationIdentifierTranscriber Instance { get; } = new _COMMA_annotationIdentifierTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._COMMA_annotationIdentifier value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdata.Trancsribers.Rules._annotationIdentifierTranscriber.Instance.Transcribe(value._annotationIdentifier_1, builder);

        }
    }
    
}
