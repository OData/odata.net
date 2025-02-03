namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _primitivePathExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._primitivePathExpr>
    {
        private _primitivePathExprTranscriber()
        {
        }
        
        public static _primitivePathExprTranscriber Instance { get; } = new _primitivePathExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._primitivePathExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Inners._ʺx2FʺTranscriber.Instance.Transcribe(value._ʺx2Fʺ_1, builder);
if (value._annotationExprⳆboundFunctionExpr_1 != null)
{
__GeneratedOdataV2.Trancsribers.Inners._annotationExprⳆboundFunctionExprTranscriber.Instance.Transcribe(value._annotationExprⳆboundFunctionExpr_1, builder);
}

        }
    }
    
}
