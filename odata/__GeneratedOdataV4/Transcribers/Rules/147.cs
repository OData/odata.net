namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _substringMethodCallExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._substringMethodCallExpr>
    {
        private _substringMethodCallExprTranscriber()
        {
        }
        
        public static _substringMethodCallExprTranscriber Instance { get; } = new _substringMethodCallExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._substringMethodCallExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Inners._ʺx73x75x62x73x74x72x69x6Ex67ʺTranscriber.Instance.Transcribe(value._ʺx73x75x62x73x74x72x69x6Ex67ʺ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_2, builder);
__GeneratedOdataV4.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_3, builder);
__GeneratedOdataV4.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_2, builder);
__GeneratedOdataV4.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_4, builder);
if (value._COMMA_BWS_commonExpr_BWS_1 != null)
{
__GeneratedOdataV4.Trancsribers.Inners._COMMA_BWS_commonExpr_BWSTranscriber.Instance.Transcribe(value._COMMA_BWS_commonExpr_BWS_1, builder);
}
__GeneratedOdataV4.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}
