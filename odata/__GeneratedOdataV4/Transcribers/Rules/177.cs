namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _eqExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._eqExpr>
    {
        private _eqExprTranscriber()
        {
        }
        
        public static _eqExprTranscriber Instance { get; } = new _eqExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._eqExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_1, builder);
__GeneratedOdataV4.Trancsribers.Inners._ʺx65x71ʺTranscriber.Instance.Transcribe(value._ʺx65x71ʺ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_2, builder);
__GeneratedOdataV4.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);

        }
    }
    
}
