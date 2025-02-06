namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _boolMethodCallExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._boolMethodCallExpr>
    {
        private _boolMethodCallExprTranscriber()
        {
        }
        
        public static _boolMethodCallExprTranscriber Instance { get; } = new _boolMethodCallExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._boolMethodCallExpr value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Rules._boolMethodCallExpr.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._boolMethodCallExpr._endsWithMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._endsWithMethodCallExprTranscriber.Instance.Transcribe(node._endsWithMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._boolMethodCallExpr._startsWithMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._startsWithMethodCallExprTranscriber.Instance.Transcribe(node._startsWithMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._boolMethodCallExpr._containsMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._containsMethodCallExprTranscriber.Instance.Transcribe(node._containsMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._boolMethodCallExpr._intersectsMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._intersectsMethodCallExprTranscriber.Instance.Transcribe(node._intersectsMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._boolMethodCallExpr._hasSubsetMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._hasSubsetMethodCallExprTranscriber.Instance.Transcribe(node._hasSubsetMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._boolMethodCallExpr._hasSubsequenceMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._hasSubsequenceMethodCallExprTranscriber.Instance.Transcribe(node._hasSubsequenceMethodCallExpr_1, context);

return default;
            }
        }
    }
    
}
