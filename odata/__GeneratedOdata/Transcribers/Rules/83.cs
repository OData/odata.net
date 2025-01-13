namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _searchAndExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._searchAndExpr>
    {
        private _searchAndExprTranscriber()
        {
        }
        
        public static _searchAndExprTranscriber Instance { get; } = new _searchAndExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._searchAndExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_1, builder);
if (value._ʺx41x4Ex44ʺ_RWS_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._ʺx41x4Ex44ʺ_RWSTranscriber.Instance.Transcribe(value._ʺx41x4Ex44ʺ_RWS_1, builder);
}
__GeneratedOdata.Trancsribers.Rules._searchExprTranscriber.Instance.Transcribe(value._searchExpr_1, builder);

        }
    }
    
}
