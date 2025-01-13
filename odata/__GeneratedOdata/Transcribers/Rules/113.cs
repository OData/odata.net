namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _contextPropertyPathTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._contextPropertyPath>
    {
        private _contextPropertyPathTranscriber()
        {
        }
        
        public static _contextPropertyPathTranscriber Instance { get; } = new _contextPropertyPathTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._contextPropertyPath value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._contextPropertyPath.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._contextPropertyPath._primitiveProperty node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._primitivePropertyTranscriber.Instance.Transcribe(node._primitiveProperty_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._contextPropertyPath._primitiveColProperty node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._primitiveColPropertyTranscriber.Instance.Transcribe(node._primitiveColProperty_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._contextPropertyPath._complexColProperty node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._complexColPropertyTranscriber.Instance.Transcribe(node._complexColProperty_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._contextPropertyPath._complexProperty_꘡꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._complexPropertyTranscriber.Instance.Transcribe(node._complexProperty_1, context);
if (node._꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPathTranscriber.Instance.Transcribe(node._꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath_1, context);
}

return default;
            }
        }
    }
    
}
