namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _boundFunctionExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._boundFunctionExpr>
    {
        private _boundFunctionExprTranscriber()
        {
        }
        
        public static _boundFunctionExprTranscriber Instance { get; } = new _boundFunctionExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._boundFunctionExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._functionExprTranscriber.Instance.Transcribe(value._functionExpr_1, builder);

        }
    }
    
}
