namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr>
    {
        private _eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExprTranscriber()
        {
        }
        
        public static _eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExprTranscriber Instance { get; } = new _eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._eqExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._eqExprTranscriber.Instance.Transcribe(node._eqExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._neExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._neExprTranscriber.Instance.Transcribe(node._neExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._ltExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._ltExprTranscriber.Instance.Transcribe(node._ltExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._leExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._leExprTranscriber.Instance.Transcribe(node._leExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._gtExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._gtExprTranscriber.Instance.Transcribe(node._gtExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._geExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._geExprTranscriber.Instance.Transcribe(node._geExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._hasExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._hasExprTranscriber.Instance.Transcribe(node._hasExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._inExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._inExprTranscriber.Instance.Transcribe(node._inExpr_1, context);

return default;
            }
        }
    }
    
}
