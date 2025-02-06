namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _listExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._listExpr>
    {
        private _listExprTranscriber()
        {
        }
        
        public static _listExprTranscriber Instance { get; } = new _listExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._listExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_2, builder);
foreach (var _ⲤCOMMA_BWS_commonExpr_BWSↃ_1 in value._ⲤCOMMA_BWS_commonExpr_BWSↃ_1)
{
Inners._ⲤCOMMA_BWS_commonExpr_BWSↃTranscriber.Instance.Transcribe(_ⲤCOMMA_BWS_commonExpr_BWSↃ_1, builder);
}
__GeneratedOdataV4.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}
