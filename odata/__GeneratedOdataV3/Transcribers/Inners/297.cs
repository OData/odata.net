namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _COMMA_BWS_commonExpr_BWSTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._COMMA_BWS_commonExpr_BWS>
    {
        private _COMMA_BWS_commonExpr_BWSTranscriber()
        {
        }
        
        public static _COMMA_BWS_commonExpr_BWSTranscriber Instance { get; } = new _COMMA_BWS_commonExpr_BWSTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._COMMA_BWS_commonExpr_BWS value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_2, builder);

        }
    }
    
}
