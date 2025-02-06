namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _anyExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._anyExpr>
    {
        private _anyExprTranscriber()
        {
        }
        
        public static _anyExprTranscriber Instance { get; } = new _anyExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._anyExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Inners._ʺx61x6Ex79ʺTranscriber.Instance.Transcribe(value._ʺx61x6Ex79ʺ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_1, builder);
if (value._lambdaVariableExpr_BWS_COLON_BWS_lambdaPredicateExpr_1 != null)
{
__GeneratedOdataV4.Trancsribers.Inners._lambdaVariableExpr_BWS_COLON_BWS_lambdaPredicateExprTranscriber.Instance.Transcribe(value._lambdaVariableExpr_BWS_COLON_BWS_lambdaPredicateExpr_1, builder);
}
__GeneratedOdataV4.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_2, builder);
__GeneratedOdataV4.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}
