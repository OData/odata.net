namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr>
    {
        private _addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExprTranscriber()
        {
        }
        
        public static _addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExprTranscriber Instance { get; } = new _addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._addExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._addExprTranscriber.Instance.Transcribe(node._addExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._subExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._subExprTranscriber.Instance.Transcribe(node._subExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._mulExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._mulExprTranscriber.Instance.Transcribe(node._mulExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._divExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._divExprTranscriber.Instance.Transcribe(node._divExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._divbyExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._divbyExprTranscriber.Instance.Transcribe(node._divbyExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._modExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._modExprTranscriber.Instance.Transcribe(node._modExpr_1, context);

return default;
            }
        }
    }
    
}
