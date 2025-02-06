namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _andExprⳆorExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._andExprⳆorExpr>
    {
        private _andExprⳆorExprTranscriber()
        {
        }
        
        public static _andExprⳆorExprTranscriber Instance { get; } = new _andExprⳆorExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._andExprⳆorExpr value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Inners._andExprⳆorExpr.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._andExprⳆorExpr._andExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._andExprTranscriber.Instance.Transcribe(node._andExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._andExprⳆorExpr._orExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._orExprTranscriber.Instance.Transcribe(node._orExpr_1, context);

return default;
            }
        }
    }
    
}
