namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _searchExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._searchExpr>
    {
        private _searchExprTranscriber()
        {
        }
        
        public static _searchExprTranscriber Instance { get; } = new _searchExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._searchExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._ⲤOPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTermↃTranscriber.Instance.Transcribe(value._ⲤOPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTermↃ_1, builder);
if (value._searchOrExprⳆsearchAndExpr_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._searchOrExprⳆsearchAndExprTranscriber.Instance.Transcribe(value._searchOrExprⳆsearchAndExpr_1, builder);
}

        }
    }
    
}
