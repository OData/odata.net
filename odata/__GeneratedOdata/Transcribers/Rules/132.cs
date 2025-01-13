namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _boundFunctionExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._boundFunctionExpr>
    {
        private _boundFunctionExprTranscriber()
        {
        }
        
        public static _boundFunctionExprTranscriber Instance { get; } = new _boundFunctionExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._boundFunctionExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._functionExprTranscriber.Instance.Transcribe(value._functionExpr_1, builder);

        }
    }
    
}
