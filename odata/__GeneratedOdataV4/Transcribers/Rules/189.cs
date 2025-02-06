namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _divbyExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._divbyExpr>
    {
        private _divbyExprTranscriber()
        {
        }
        
        public static _divbyExprTranscriber Instance { get; } = new _divbyExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._divbyExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_1, builder);
__GeneratedOdataV4.Trancsribers.Inners._ʺx64x69x76x62x79ʺTranscriber.Instance.Transcribe(value._ʺx64x69x76x62x79ʺ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_2, builder);
__GeneratedOdataV4.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);

        }
    }
    
}
