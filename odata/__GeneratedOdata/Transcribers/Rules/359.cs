namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _annotationsListTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._annotationsList>
    {
        private _annotationsListTranscriber()
        {
        }
        
        public static _annotationsListTranscriber Instance { get; } = new _annotationsListTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._annotationsList value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._annotationIdentifierTranscriber.Instance.Transcribe(value._annotationIdentifier_1, builder);
foreach (var _ⲤCOMMA_annotationIdentifierↃ_1 in value._ⲤCOMMA_annotationIdentifierↃ_1)
{
__GeneratedOdata.Trancsribers.Inners._ⲤCOMMA_annotationIdentifierↃTranscriber.Instance.Transcribe(_ⲤCOMMA_annotationIdentifierↃ_1, builder);
}

        }
    }
    
}