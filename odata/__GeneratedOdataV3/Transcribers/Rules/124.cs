namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _implicitVariableExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._implicitVariableExpr>
    {
        private _implicitVariableExprTranscriber()
        {
        }
        
        public static _implicitVariableExprTranscriber Instance { get; } = new _implicitVariableExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._implicitVariableExpr value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Rules._implicitVariableExpr.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._implicitVariableExpr._ʺx24x69x74ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx24x69x74ʺTranscriber.Instance.Transcribe(node._ʺx24x69x74ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._implicitVariableExpr._ʺx24x74x68x69x73ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx24x74x68x69x73ʺTranscriber.Instance.Transcribe(node._ʺx24x74x68x69x73ʺ_1, context);

return default;
            }
        }
    }
    
}
