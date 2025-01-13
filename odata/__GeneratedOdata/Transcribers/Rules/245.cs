namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _primitivePropertyTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._primitiveProperty>
    {
        private _primitivePropertyTranscriber()
        {
        }
        
        public static _primitivePropertyTranscriber Instance { get; } = new _primitivePropertyTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._primitiveProperty value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._primitiveProperty.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._primitiveProperty._primitiveKeyProperty node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._primitiveKeyPropertyTranscriber.Instance.Transcribe(node._primitiveKeyProperty_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._primitiveProperty._primitiveNonKeyProperty node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._primitiveNonKeyPropertyTranscriber.Instance.Transcribe(node._primitiveNonKeyProperty_1, context);

return default;
            }
        }
    }
    
}
