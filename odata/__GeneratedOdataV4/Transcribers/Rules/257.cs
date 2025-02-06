namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _functionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._function>
    {
        private _functionTranscriber()
        {
        }
        
        public static _functionTranscriber Instance { get; } = new _functionTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._function value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Rules._function.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._function._entityFunction node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._entityFunctionTranscriber.Instance.Transcribe(node._entityFunction_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._function._entityColFunction node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._entityColFunctionTranscriber.Instance.Transcribe(node._entityColFunction_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._function._complexFunction node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._complexFunctionTranscriber.Instance.Transcribe(node._complexFunction_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._function._complexColFunction node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._complexColFunctionTranscriber.Instance.Transcribe(node._complexColFunction_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._function._primitiveFunction node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._primitiveFunctionTranscriber.Instance.Transcribe(node._primitiveFunction_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._function._primitiveColFunction node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._primitiveColFunctionTranscriber.Instance.Transcribe(node._primitiveColFunction_1, context);

return default;
            }
        }
    }
    
}
