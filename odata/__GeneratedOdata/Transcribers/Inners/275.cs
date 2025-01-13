namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr>
    {
        private _keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExprTranscriber()
        {
        }
        
        public static _keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExprTranscriber Instance { get; } = new _keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr._keyPredicate_꘡singleNavigationExpr꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._keyPredicateTranscriber.Instance.Transcribe(node._keyPredicate_1, context);
if (node._singleNavigationExpr_1 != null)
{
__GeneratedOdata.Trancsribers.Rules._singleNavigationExprTranscriber.Instance.Transcribe(node._singleNavigationExpr_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr._collectionPathExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._collectionPathExprTranscriber.Instance.Transcribe(node._collectionPathExpr_1, context);

return default;
            }
        }
    }
    
}
