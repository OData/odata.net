namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _rootExprColTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._rootExprCol>
    {
        private _rootExprColTranscriber()
        {
        }
        
        public static _rootExprColTranscriber Instance { get; } = new _rootExprColTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._rootExprCol value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._beginⲻarrayTranscriber.Instance.Transcribe(value._beginⲻarray_1, builder);
if (value._rootExpr_ЖⲤvalueⲻseparator_rootExprↃ_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._rootExpr_ЖⲤvalueⲻseparator_rootExprↃTranscriber.Instance.Transcribe(value._rootExpr_ЖⲤvalueⲻseparator_rootExprↃ_1, builder);
}
__GeneratedOdata.Trancsribers.Rules._endⲻarrayTranscriber.Instance.Transcribe(value._endⲻarray_1, builder);

        }
    }
    
}