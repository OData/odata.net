namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _eqExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._eqExpr>
    {
        private _eqExprTranscriber()
        {
        }
        
        public static _eqExprTranscriber Instance { get; } = new _eqExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._eqExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx65x71ʺTranscriber.Instance.Transcribe(value._ʺx65x71ʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_2, builder);
__GeneratedOdata.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);

        }
    }
    
}
