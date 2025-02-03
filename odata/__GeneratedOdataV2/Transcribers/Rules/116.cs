namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _rootExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._rootExpr>
    {
        private _rootExprTranscriber()
        {
        }
        
        public static _rootExprTranscriber Instance { get; } = new _rootExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._rootExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Inners._ʺx24x72x6Fx6Fx74x2FʺTranscriber.Instance.Transcribe(value._ʺx24x72x6Fx6Fx74x2Fʺ_1, builder);
__GeneratedOdataV2.Trancsribers.Inners._ⲤentitySetName_keyPredicateⳆsingletonEntityↃTranscriber.Instance.Transcribe(value._ⲤentitySetName_keyPredicateⳆsingletonEntityↃ_1, builder);
if (value._singleNavigationExpr_1 != null)
{
__GeneratedOdataV2.Trancsribers.Rules._singleNavigationExprTranscriber.Instance.Transcribe(value._singleNavigationExpr_1, builder);
}

        }
    }
    
}
