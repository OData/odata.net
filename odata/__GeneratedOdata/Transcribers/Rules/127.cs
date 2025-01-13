namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _singleNavigationExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._singleNavigationExpr>
    {
        private _singleNavigationExprTranscriber()
        {
        }
        
        public static _singleNavigationExprTranscriber Instance { get; } = new _singleNavigationExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._singleNavigationExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._ʺx2FʺTranscriber.Instance.Transcribe(value._ʺx2Fʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._memberExprTranscriber.Instance.Transcribe(value._memberExpr_1, builder);

        }
    }
    
}
