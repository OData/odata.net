namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _searchOrExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._searchOrExpr>
    {
        private _searchOrExprTranscriber()
        {
        }
        
        public static _searchOrExprTranscriber Instance { get; } = new _searchOrExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._searchOrExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_1, builder);
__GeneratedOdataV3.Trancsribers.Inners._ʺx4Fx52ʺTranscriber.Instance.Transcribe(value._ʺx4Fx52ʺ_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_2, builder);
__GeneratedOdataV3.Trancsribers.Rules._searchExprTranscriber.Instance.Transcribe(value._searchExpr_1, builder);

        }
    }
    
}
