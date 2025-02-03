namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _lambdaPredicateExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._lambdaPredicateExpr>
    {
        private _lambdaPredicateExprTranscriber()
        {
        }
        
        public static _lambdaPredicateExprTranscriber Instance { get; } = new _lambdaPredicateExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._lambdaPredicateExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._boolCommonExprTranscriber.Instance.Transcribe(value._boolCommonExpr_1, builder);

        }
    }
    
}
