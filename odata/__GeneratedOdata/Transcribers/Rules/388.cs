namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _hostTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._host>
    {
        private _hostTranscriber()
        {
        }
        
        public static _hostTranscriber Instance { get; } = new _hostTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._host value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._host.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._host._IPⲻliteral node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._IPⲻliteralTranscriber.Instance.Transcribe(node._IPⲻliteral_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._host._IPv4address node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._IPv4addressTranscriber.Instance.Transcribe(node._IPv4address_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._host._regⲻname node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._regⲻnameTranscriber.Instance.Transcribe(node._regⲻname_1, context);

return default;
            }
        }
    }
    
}
