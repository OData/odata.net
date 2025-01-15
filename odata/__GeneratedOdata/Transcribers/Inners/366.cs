namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _rootExpr_ЖⲤvalueⲻseparator_rootExprↃTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._rootExpr_ЖⲤvalueⲻseparator_rootExprↃ>
    {
        private _rootExpr_ЖⲤvalueⲻseparator_rootExprↃTranscriber()
        {
        }
        
        public static _rootExpr_ЖⲤvalueⲻseparator_rootExprↃTranscriber Instance { get; } = new _rootExpr_ЖⲤvalueⲻseparator_rootExprↃTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._rootExpr_ЖⲤvalueⲻseparator_rootExprↃ value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._rootExprTranscriber.Instance.Transcribe(value._rootExpr_1, builder);
foreach (var _Ⲥvalueⲻseparator_rootExprↃ_1 in value._Ⲥvalueⲻseparator_rootExprↃ_1)
{
Inners._Ⲥvalueⲻseparator_rootExprↃTranscriber.Instance.Transcribe(_Ⲥvalueⲻseparator_rootExprↃ_1, builder);
}

        }
    }
    
}
