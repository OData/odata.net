namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _searchOrExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._searchOrExpr>
    {
        private _searchOrExprTranscriber()
        {
        }
        
        public static _searchOrExprTranscriber Instance { get; } = new _searchOrExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._searchOrExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_1, builder);
__GeneratedOdataV4.Trancsribers.Inners._ʺx4Fx52ʺTranscriber.Instance.Transcribe(value._ʺx4Fx52ʺ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_2, builder);
__GeneratedOdataV4.Trancsribers.Rules._searchExprTranscriber.Instance.Transcribe(value._searchExpr_1, builder);

        }
    }
    
}
