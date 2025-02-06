namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _allExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._allExpr>
    {
        private _allExprTranscriber()
        {
        }
        
        public static _allExprTranscriber Instance { get; } = new _allExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._allExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Inners._ʺx61x6Cx6CʺTranscriber.Instance.Transcribe(value._ʺx61x6Cx6Cʺ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._lambdaVariableExprTranscriber.Instance.Transcribe(value._lambdaVariableExpr_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_2, builder);
__GeneratedOdataV4.Trancsribers.Rules._COLONTranscriber.Instance.Transcribe(value._COLON_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_3, builder);
__GeneratedOdataV4.Trancsribers.Rules._lambdaPredicateExprTranscriber.Instance.Transcribe(value._lambdaPredicateExpr_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_4, builder);
__GeneratedOdataV4.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}
