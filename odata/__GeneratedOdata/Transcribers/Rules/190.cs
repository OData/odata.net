namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _modExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._modExpr>
    {
        private _modExprTranscriber()
        {
        }
        
        public static _modExprTranscriber Instance { get; } = new _modExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._modExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx6Dx6Fx64ʺTranscriber.Instance.Transcribe(value._ʺx6Dx6Fx64ʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_2, builder);
__GeneratedOdata.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);

        }
    }
    
}
