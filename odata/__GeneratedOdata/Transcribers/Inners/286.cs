namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _lambdaVariableExpr_BWS_COLON_BWS_lambdaPredicateExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._lambdaVariableExpr_BWS_COLON_BWS_lambdaPredicateExpr>
    {
        private _lambdaVariableExpr_BWS_COLON_BWS_lambdaPredicateExprTranscriber()
        {
        }
        
        public static _lambdaVariableExpr_BWS_COLON_BWS_lambdaPredicateExprTranscriber Instance { get; } = new _lambdaVariableExpr_BWS_COLON_BWS_lambdaPredicateExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._lambdaVariableExpr_BWS_COLON_BWS_lambdaPredicateExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._lambdaVariableExprTranscriber.Instance.Transcribe(value._lambdaVariableExpr_1, builder);
__GeneratedOdata.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_1, builder);
__GeneratedOdata.Trancsribers.Rules._COLONTranscriber.Instance.Transcribe(value._COLON_1, builder);
__GeneratedOdata.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_2, builder);
__GeneratedOdata.Trancsribers.Rules._lambdaPredicateExprTranscriber.Instance.Transcribe(value._lambdaPredicateExpr_1, builder);

        }
    }
    
}
