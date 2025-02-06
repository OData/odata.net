namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _annotationIdentifierTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._annotationIdentifier>
    {
        private _annotationIdentifierTranscriber()
        {
        }
        
        public static _annotationIdentifierTranscriber Instance { get; } = new _annotationIdentifierTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._annotationIdentifier value, System.Text.StringBuilder builder)
        {
            if (value._excludeOperator_1 != null)
{
__GeneratedOdataV4.Trancsribers.Rules._excludeOperatorTranscriber.Instance.Transcribe(value._excludeOperator_1, builder);
}
__GeneratedOdataV4.Trancsribers.Inners._ⲤSTARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃↃTranscriber.Instance.Transcribe(value._ⲤSTARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃↃ_1, builder);
if (value._ʺx23ʺ_odataIdentifier_1 != null)
{
__GeneratedOdataV4.Trancsribers.Inners._ʺx23ʺ_odataIdentifierTranscriber.Instance.Transcribe(value._ʺx23ʺ_odataIdentifier_1, builder);
}

        }
    }
    
}
