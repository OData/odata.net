namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _primitivePathExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._primitivePathExpr>
    {
        private _primitivePathExprTranscriber()
        {
        }
        
        public static _primitivePathExprTranscriber Instance { get; } = new _primitivePathExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._primitivePathExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._ʺx2FʺTranscriber.Instance.Transcribe(value._ʺx2Fʺ_1, builder);
if (value._annotationExprⳆboundFunctionExpr_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._annotationExprⳆboundFunctionExprTranscriber.Instance.Transcribe(value._annotationExprⳆboundFunctionExpr_1, builder);
}

        }
    }
    
}
