namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _commonExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._commonExpr>
    {
        private _commonExprTranscriber()
        {
        }
        
        public static _commonExprTranscriber Instance { get; } = new _commonExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._commonExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._ⲤprimitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprↃTranscriber.Instance.Transcribe(value._ⲤprimitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprↃ_1, builder);
if (value._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExprTranscriber.Instance.Transcribe(value._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr_1, builder);
}
if (value._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExprTranscriber.Instance.Transcribe(value._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr_1, builder);
}
if (value._andExprⳆorExpr_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._andExprⳆorExprTranscriber.Instance.Transcribe(value._andExprⳆorExpr_1, builder);
}

        }
    }
    
}
