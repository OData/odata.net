namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _complexPropertyⳆcomplexColPropertyTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._complexPropertyⳆcomplexColProperty>
    {
        private _complexPropertyⳆcomplexColPropertyTranscriber()
        {
        }
        
        public static _complexPropertyⳆcomplexColPropertyTranscriber Instance { get; } = new _complexPropertyⳆcomplexColPropertyTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._complexPropertyⳆcomplexColProperty value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._complexPropertyⳆcomplexColProperty.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._complexPropertyⳆcomplexColProperty._complexProperty node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._complexPropertyTranscriber.Instance.Transcribe(node._complexProperty_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._complexPropertyⳆcomplexColProperty._complexColProperty node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._complexColPropertyTranscriber.Instance.Transcribe(node._complexColProperty_1, context);

return default;
            }
        }
    }
    
}