namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _listExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._listExpr>
    {
        private _listExprTranscriber()
        {
        }
        
        public static _listExprTranscriber Instance { get; } = new _listExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._listExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdata.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_1, builder);
__GeneratedOdata.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);
__GeneratedOdata.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_2, builder);
foreach (var _ⲤCOMMA_BWS_commonExpr_BWSↃ_1 in value._ⲤCOMMA_BWS_commonExpr_BWSↃ_1)
{
__GeneratedOdata.Trancsribers.Inners._ⲤCOMMA_BWS_commonExpr_BWSↃTranscriber.Instance.Transcribe(_ⲤCOMMA_BWS_commonExpr_BWSↃ_1, builder);
}
__GeneratedOdata.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}
