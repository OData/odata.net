namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _firstMemberExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._firstMemberExpr>
    {
        private _firstMemberExprTranscriber()
        {
        }
        
        public static _firstMemberExprTranscriber Instance { get; } = new _firstMemberExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._firstMemberExpr value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._firstMemberExpr.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._firstMemberExpr._memberExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._memberExprTranscriber.Instance.Transcribe(node._memberExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._firstMemberExpr._inscopeVariableExpr_꘡ʺx2Fʺ_memberExpr꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._inscopeVariableExprTranscriber.Instance.Transcribe(node._inscopeVariableExpr_1, context);
if (node._ʺx2Fʺ_memberExpr_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._ʺx2Fʺ_memberExprTranscriber.Instance.Transcribe(node._ʺx2Fʺ_memberExpr_1, context);
}

return default;
            }
        }
    }
    
}
