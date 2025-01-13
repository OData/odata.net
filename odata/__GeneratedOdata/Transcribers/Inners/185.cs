namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTermTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm>
    {
        private _OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTermTranscriber()
        {
        }
        
        public static _OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTermTranscriber Instance { get; } = new _OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTermTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm._OPEN_BWS_searchExpr_BWS_CLOSE node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(node._OPEN_1, context);
__GeneratedOdata.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(node._BWS_1, context);
__GeneratedOdata.Trancsribers.Rules._searchExprTranscriber.Instance.Transcribe(node._searchExpr_1, context);
__GeneratedOdata.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(node._BWS_2, context);
__GeneratedOdata.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(node._CLOSE_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm._searchTerm node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._searchTermTranscriber.Instance.Transcribe(node._searchTerm_1, context);

return default;
            }
        }
    }
    
}
