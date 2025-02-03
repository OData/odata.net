namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _inscopeVariableExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._inscopeVariableExpr>
    {
        private _inscopeVariableExprTranscriber()
        {
        }
        
        public static _inscopeVariableExprTranscriber Instance { get; } = new _inscopeVariableExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._inscopeVariableExpr value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV2.CstNodes.Rules._inscopeVariableExpr.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._inscopeVariableExpr._implicitVariableExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._implicitVariableExprTranscriber.Instance.Transcribe(node._implicitVariableExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._inscopeVariableExpr._parameterAlias node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._parameterAliasTranscriber.Instance.Transcribe(node._parameterAlias_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._inscopeVariableExpr._lambdaVariableExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._lambdaVariableExprTranscriber.Instance.Transcribe(node._lambdaVariableExpr_1, context);

return default;
            }
        }
    }
    
}
