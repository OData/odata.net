namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _negateExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._negateExpr>
    {
        private _negateExprTranscriber()
        {
        }
        
        public static _negateExprTranscriber Instance { get; } = new _negateExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._negateExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._ʺx2DʺTranscriber.Instance.Transcribe(value._ʺx2Dʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_1, builder);
__GeneratedOdata.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);

        }
    }
    
}
