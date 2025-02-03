namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr>
    {
        private _collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExprTranscriber()
        {
        }
        
        public static _collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExprTranscriber Instance { get; } = new _collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._collectionPathExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._collectionPathExprTranscriber.Instance.Transcribe(node._collectionPathExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._singleNavigationExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._singleNavigationExprTranscriber.Instance.Transcribe(node._singleNavigationExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._complexPathExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._complexPathExprTranscriber.Instance.Transcribe(node._complexPathExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._primitivePathExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._primitivePathExprTranscriber.Instance.Transcribe(node._primitivePathExpr_1, context);

return default;
            }
        }
    }
    
}
