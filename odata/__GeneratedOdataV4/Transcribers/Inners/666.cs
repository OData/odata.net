namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _unreservedⳆpctⲻencodedⳆsubⲻdelimsTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims>
    {
        private _unreservedⳆpctⲻencodedⳆsubⲻdelimsTranscriber()
        {
        }
        
        public static _unreservedⳆpctⲻencodedⳆsubⲻdelimsTranscriber Instance { get; } = new _unreservedⳆpctⲻencodedⳆsubⲻdelimsTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._unreserved node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._unreservedTranscriber.Instance.Transcribe(node._unreserved_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._pctⲻencoded node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._pctⲻencodedTranscriber.Instance.Transcribe(node._pctⲻencoded_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._subⲻdelims node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._subⲻdelimsTranscriber.Instance.Transcribe(node._subⲻdelims_1, context);

return default;
            }
        }
    }
    
}
