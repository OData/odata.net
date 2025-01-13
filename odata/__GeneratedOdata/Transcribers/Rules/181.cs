namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _gtExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._gtExpr>
    {
        private _gtExprTranscriber()
        {
        }
        
        public static _gtExprTranscriber Instance { get; } = new _gtExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._gtExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx67x74ʺTranscriber.Instance.Transcribe(value._ʺx67x74ʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_2, builder);
__GeneratedOdata.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);

        }
    }
    
}
