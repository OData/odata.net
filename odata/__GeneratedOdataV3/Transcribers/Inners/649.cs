namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _unreservedⳆsubⲻdelimsⳆʺx3AʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ>
    {
        private _unreservedⳆsubⲻdelimsⳆʺx3AʺTranscriber()
        {
        }
        
        public static _unreservedⳆsubⲻdelimsⳆʺx3AʺTranscriber Instance { get; } = new _unreservedⳆsubⲻdelimsⳆʺx3AʺTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ._unreserved node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._unreservedTranscriber.Instance.Transcribe(node._unreserved_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ._subⲻdelims node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._subⲻdelimsTranscriber.Instance.Transcribe(node._subⲻdelims_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ._ʺx3Aʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx3AʺTranscriber.Instance.Transcribe(node._ʺx3Aʺ_1, context);

return default;
            }
        }
    }
    
}
