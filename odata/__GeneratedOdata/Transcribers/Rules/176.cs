namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _orExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._orExpr>
    {
        private _orExprTranscriber()
        {
        }
        
        public static _orExprTranscriber Instance { get; } = new _orExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._orExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx6Fx72ʺTranscriber.Instance.Transcribe(value._ʺx6Fx72ʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_2, builder);
__GeneratedOdata.Trancsribers.Rules._boolCommonExprTranscriber.Instance.Transcribe(value._boolCommonExpr_1, builder);

        }
    }
    
}
