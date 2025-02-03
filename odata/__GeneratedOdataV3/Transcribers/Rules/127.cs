namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _singleNavigationExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._singleNavigationExpr>
    {
        private _singleNavigationExprTranscriber()
        {
        }
        
        public static _singleNavigationExprTranscriber Instance { get; } = new _singleNavigationExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._singleNavigationExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Inners._ʺx2FʺTranscriber.Instance.Transcribe(value._ʺx2Fʺ_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._memberExprTranscriber.Instance.Transcribe(value._memberExpr_1, builder);

        }
    }
    
}
