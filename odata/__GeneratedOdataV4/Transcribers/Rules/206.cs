namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _rootExprColTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._rootExprCol>
    {
        private _rootExprColTranscriber()
        {
        }
        
        public static _rootExprColTranscriber Instance { get; } = new _rootExprColTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._rootExprCol value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._beginⲻarrayTranscriber.Instance.Transcribe(value._beginⲻarray_1, builder);
if (value._rootExpr_ЖⲤvalueⲻseparator_rootExprↃ_1 != null)
{
__GeneratedOdataV4.Trancsribers.Inners._rootExpr_ЖⲤvalueⲻseparator_rootExprↃTranscriber.Instance.Transcribe(value._rootExpr_ЖⲤvalueⲻseparator_rootExprↃ_1, builder);
}
__GeneratedOdataV4.Trancsribers.Rules._endⲻarrayTranscriber.Instance.Transcribe(value._endⲻarray_1, builder);

        }
    }
    
}
