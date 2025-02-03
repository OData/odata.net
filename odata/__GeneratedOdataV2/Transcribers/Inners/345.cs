namespace __GeneratedOdataV2.Trancsribers.Inners
{
    public sealed class _commonExpr_BWS_COMMA_BWSTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Inners._commonExpr_BWS_COMMA_BWS>
    {
        private _commonExpr_BWS_COMMA_BWSTranscriber()
        {
        }
        
        public static _commonExpr_BWS_COMMA_BWSTranscriber Instance { get; } = new _commonExpr_BWS_COMMA_BWSTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Inners._commonExpr_BWS_COMMA_BWS value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_2, builder);

        }
    }
    
}
