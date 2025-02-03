namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr>
    {
        private _primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprTranscriber()
        {
        }
        
        public static _primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprTranscriber Instance { get; } = new _primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._primitiveLiteral node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._primitiveLiteralTranscriber.Instance.Transcribe(node._primitiveLiteral_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._arrayOrObject node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._arrayOrObjectTranscriber.Instance.Transcribe(node._arrayOrObject_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._rootExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._rootExprTranscriber.Instance.Transcribe(node._rootExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._firstMemberExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._firstMemberExprTranscriber.Instance.Transcribe(node._firstMemberExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._functionExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._functionExprTranscriber.Instance.Transcribe(node._functionExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._negateExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._negateExprTranscriber.Instance.Transcribe(node._negateExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._methodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._methodCallExprTranscriber.Instance.Transcribe(node._methodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._parenExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._parenExprTranscriber.Instance.Transcribe(node._parenExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._listExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._listExprTranscriber.Instance.Transcribe(node._listExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._castExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._castExprTranscriber.Instance.Transcribe(node._castExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._isofExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._isofExprTranscriber.Instance.Transcribe(node._isofExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._notExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._notExprTranscriber.Instance.Transcribe(node._notExpr_1, context);

return default;
            }
        }
    }
    
}
