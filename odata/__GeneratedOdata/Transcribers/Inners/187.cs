namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _searchOrExprⳆsearchAndExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._searchOrExprⳆsearchAndExpr>
    {
        private _searchOrExprⳆsearchAndExprTranscriber()
        {
        }
        
        public static _searchOrExprⳆsearchAndExprTranscriber Instance { get; } = new _searchOrExprⳆsearchAndExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._searchOrExprⳆsearchAndExpr value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._searchOrExprⳆsearchAndExpr.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._searchOrExprⳆsearchAndExpr._searchOrExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._searchOrExprTranscriber.Instance.Transcribe(node._searchOrExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._searchOrExprⳆsearchAndExpr._searchAndExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._searchAndExprTranscriber.Instance.Transcribe(node._searchAndExpr_1, context);

return default;
            }
        }
    }
    
}