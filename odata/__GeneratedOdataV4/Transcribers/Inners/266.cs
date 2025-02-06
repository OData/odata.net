namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _propertyPathExprⳆboundFunctionExprⳆannotationExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr>
    {
        private _propertyPathExprⳆboundFunctionExprⳆannotationExprTranscriber()
        {
        }
        
        public static _propertyPathExprⳆboundFunctionExprⳆannotationExprTranscriber Instance { get; } = new _propertyPathExprⳆboundFunctionExprⳆannotationExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr._propertyPathExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._propertyPathExprTranscriber.Instance.Transcribe(node._propertyPathExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr._boundFunctionExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._boundFunctionExprTranscriber.Instance.Transcribe(node._boundFunctionExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr._annotationExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._annotationExprTranscriber.Instance.Transcribe(node._annotationExpr_1, context);

return default;
            }
        }
    }
    
}
