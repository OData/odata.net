namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _navigationPropertyTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._navigationProperty>
    {
        private _navigationPropertyTranscriber()
        {
        }
        
        public static _navigationPropertyTranscriber Instance { get; } = new _navigationPropertyTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._navigationProperty value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._navigationProperty.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._navigationProperty._entityNavigationProperty node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._entityNavigationPropertyTranscriber.Instance.Transcribe(node._entityNavigationProperty_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._navigationProperty._entityColNavigationProperty node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._entityColNavigationPropertyTranscriber.Instance.Transcribe(node._entityColNavigationProperty_1, context);

return default;
            }
        }
    }
    
}