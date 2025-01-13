namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _geExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._geExpr>
    {
        private _geExprTranscriber()
        {
        }
        
        public static _geExprTranscriber Instance { get; } = new _geExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._geExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx67x65ʺTranscriber.Instance.Transcribe(value._ʺx67x65ʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_2, builder);
__GeneratedOdata.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);

        }
    }
    
}
