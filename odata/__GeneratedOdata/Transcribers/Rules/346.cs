namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _headerTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._header>
    {
        private _headerTranscriber()
        {
        }
        
        public static _headerTranscriber Instance { get; } = new _headerTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._header value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._header.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._header._contentⲻid node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._contentⲻidTranscriber.Instance.Transcribe(node._contentⲻid_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._header._entityid node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._entityidTranscriber.Instance.Transcribe(node._entityid_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._header._isolation node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._isolationTranscriber.Instance.Transcribe(node._isolation_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._header._odataⲻmaxversion node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._odataⲻmaxversionTranscriber.Instance.Transcribe(node._odataⲻmaxversion_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._header._odataⲻversion node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._odataⲻversionTranscriber.Instance.Transcribe(node._odataⲻversion_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._header._prefer node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._preferTranscriber.Instance.Transcribe(node._prefer_1, context);

return default;
            }
        }
    }
    
}
