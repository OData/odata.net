namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _annotationExprⳆboundFunctionExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._annotationExprⳆboundFunctionExpr>
    {
        private _annotationExprⳆboundFunctionExprTranscriber()
        {
        }
        
        public static _annotationExprⳆboundFunctionExprTranscriber Instance { get; } = new _annotationExprⳆboundFunctionExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._annotationExprⳆboundFunctionExpr value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._annotationExprⳆboundFunctionExpr.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._annotationExprⳆboundFunctionExpr._annotationExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._annotationExprTranscriber.Instance.Transcribe(node._annotationExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._annotationExprⳆboundFunctionExpr._boundFunctionExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._boundFunctionExprTranscriber.Instance.Transcribe(node._boundFunctionExpr_1, context);

return default;
            }
        }
    }
    
}
