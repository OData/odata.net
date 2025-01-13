namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _annotationIdentifierTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._annotationIdentifier>
    {
        private _annotationIdentifierTranscriber()
        {
        }
        
        public static _annotationIdentifierTranscriber Instance { get; } = new _annotationIdentifierTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._annotationIdentifier value, System.Text.StringBuilder builder)
        {
            if (value._excludeOperator_1 != null)
{
__GeneratedOdata.Trancsribers.Rules._excludeOperatorTranscriber.Instance.Transcribe(value._excludeOperator_1, builder);
}
__GeneratedOdata.Trancsribers.Inners._ⲤSTARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃↃTranscriber.Instance.Transcribe(value._ⲤSTARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃↃ_1, builder);
if (value._ʺx23ʺ_odataIdentifier_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._ʺx23ʺ_odataIdentifierTranscriber.Instance.Transcribe(value._ʺx23ʺ_odataIdentifier_1, builder);
}

        }
    }
    
}
