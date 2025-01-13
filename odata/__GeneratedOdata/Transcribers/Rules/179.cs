namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _ltExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._ltExpr>
    {
        private _ltExprTranscriber()
        {
        }
        
        public static _ltExprTranscriber Instance { get; } = new _ltExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._ltExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx6Cx74ʺTranscriber.Instance.Transcribe(value._ʺx6Cx74ʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_2, builder);
__GeneratedOdata.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);

        }
    }
    
}
