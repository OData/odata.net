namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _lambdaPredicateExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._lambdaPredicateExpr>
    {
        private _lambdaPredicateExprTranscriber()
        {
        }
        
        public static _lambdaPredicateExprTranscriber Instance { get; } = new _lambdaPredicateExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._lambdaPredicateExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._boolCommonExprTranscriber.Instance.Transcribe(value._boolCommonExpr_1, builder);

        }
    }
    
}
