namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3AʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ>
    {
        private _unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3AʺTranscriber()
        {
        }
        
        public static _unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3AʺTranscriber Instance { get; } = new _unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3AʺTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ._unreserved node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._unreservedTranscriber.Instance.Transcribe(node._unreserved_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ._pctⲻencoded node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._pctⲻencodedTranscriber.Instance.Transcribe(node._pctⲻencoded_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ._subⲻdelims node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._subⲻdelimsTranscriber.Instance.Transcribe(node._subⲻdelims_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ._ʺx3Aʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx3AʺTranscriber.Instance.Transcribe(node._ʺx3Aʺ_1, context);

return default;
            }
        }
    }
    
}
