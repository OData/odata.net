namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _leExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._leExpr>
    {
        private _leExprTranscriber()
        {
        }
        
        public static _leExprTranscriber Instance { get; } = new _leExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._leExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_1, builder);
__GeneratedOdataV4.Trancsribers.Inners._ʺx6Cx65ʺTranscriber.Instance.Transcribe(value._ʺx6Cx65ʺ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_2, builder);
__GeneratedOdataV4.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);

        }
    }
    
}
