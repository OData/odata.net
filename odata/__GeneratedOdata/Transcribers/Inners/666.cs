namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _unreservedⳆpctⲻencodedⳆsubⲻdelimsTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims>
    {
        private _unreservedⳆpctⲻencodedⳆsubⲻdelimsTranscriber()
        {
        }
        
        public static _unreservedⳆpctⲻencodedⳆsubⲻdelimsTranscriber Instance { get; } = new _unreservedⳆpctⲻencodedⳆsubⲻdelimsTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._unreserved node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._unreservedTranscriber.Instance.Transcribe(node._unreserved_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._pctⲻencoded node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._pctⲻencodedTranscriber.Instance.Transcribe(node._pctⲻencoded_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._subⲻdelims node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._subⲻdelimsTranscriber.Instance.Transcribe(node._subⲻdelims_1, context);

return default;
            }
        }
    }
    
}