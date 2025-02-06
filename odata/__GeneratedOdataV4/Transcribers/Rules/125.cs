namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _lambdaVariableExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._lambdaVariableExpr>
    {
        private _lambdaVariableExprTranscriber()
        {
        }
        
        public static _lambdaVariableExprTranscriber Instance { get; } = new _lambdaVariableExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._lambdaVariableExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
