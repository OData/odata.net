namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _searchOrExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._searchOrExpr>
    {
        private _searchOrExprTranscriber()
        {
        }
        
        public static _searchOrExprTranscriber Instance { get; } = new _searchOrExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._searchOrExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx4Fx52ʺTranscriber.Instance.Transcribe(value._ʺx4Fx52ʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_2, builder);
__GeneratedOdata.Trancsribers.Rules._searchExprTranscriber.Instance.Transcribe(value._searchExpr_1, builder);

        }
    }
    
}
