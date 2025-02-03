namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _trimMethodCallExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._trimMethodCallExpr>
    {
        private _trimMethodCallExprTranscriber()
        {
        }
        
        public static _trimMethodCallExprTranscriber Instance { get; } = new _trimMethodCallExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._trimMethodCallExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Inners._ʺx74x72x69x6DʺTranscriber.Instance.Transcribe(value._ʺx74x72x69x6Dʺ_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_2, builder);
__GeneratedOdataV2.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}
