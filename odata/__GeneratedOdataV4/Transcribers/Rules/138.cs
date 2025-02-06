namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _lambdaPredicateExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._lambdaPredicateExpr>
    {
        private _lambdaPredicateExprTranscriber()
        {
        }
        
        public static _lambdaPredicateExprTranscriber Instance { get; } = new _lambdaPredicateExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._lambdaPredicateExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._boolCommonExprTranscriber.Instance.Transcribe(value._boolCommonExpr_1, builder);

        }
    }
    
}
