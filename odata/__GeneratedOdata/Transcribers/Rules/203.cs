namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _navigationPropertyInUriTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._navigationPropertyInUri>
    {
        private _navigationPropertyInUriTranscriber()
        {
        }
        
        public static _navigationPropertyInUriTranscriber Instance { get; } = new _navigationPropertyInUriTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._navigationPropertyInUri value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._navigationPropertyInUri.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._navigationPropertyInUri._singleNavPropInJSON node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._singleNavPropInJSONTranscriber.Instance.Transcribe(node._singleNavPropInJSON_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._navigationPropertyInUri._collectionNavPropInJSON node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._collectionNavPropInJSONTranscriber.Instance.Transcribe(node._collectionNavPropInJSON_1, context);

return default;
            }
        }
    }
    
}
