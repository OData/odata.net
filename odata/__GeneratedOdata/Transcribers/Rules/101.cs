namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _parameterValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._parameterValue>
    {
        private _parameterValueTranscriber()
        {
        }
        
        public static _parameterValueTranscriber Instance { get; } = new _parameterValueTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._parameterValue value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._parameterValue.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._parameterValue._arrayOrObject node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._arrayOrObjectTranscriber.Instance.Transcribe(node._arrayOrObject_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._parameterValue._commonExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(node._commonExpr_1, context);

return default;
            }
        }
    }
    
}