namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _ceilingMethodCallExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._ceilingMethodCallExpr>
    {
        private _ceilingMethodCallExprTranscriber()
        {
        }
        
        public static _ceilingMethodCallExprTranscriber Instance { get; } = new _ceilingMethodCallExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._ceilingMethodCallExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Inners._ʺx63x65x69x6Cx69x6Ex67ʺTranscriber.Instance.Transcribe(value._ʺx63x65x69x6Cx69x6Ex67ʺ_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_2, builder);
__GeneratedOdataV2.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}
