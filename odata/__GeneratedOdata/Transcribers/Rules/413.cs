namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _qcharⲻnoⲻAMPⲻEQTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQ>
    {
        private _qcharⲻnoⲻAMPⲻEQTranscriber()
        {
        }
        
        public static _qcharⲻnoⲻAMPⲻEQTranscriber Instance { get; } = new _qcharⲻnoⲻAMPⲻEQTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQ._unreserved node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._unreservedTranscriber.Instance.Transcribe(node._unreserved_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQ._pctⲻencoded node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._pctⲻencodedTranscriber.Instance.Transcribe(node._pctⲻencoded_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQ._otherⲻdelims node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._otherⲻdelimsTranscriber.Instance.Transcribe(node._otherⲻdelims_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQ._ʺx3Aʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx3AʺTranscriber.Instance.Transcribe(node._ʺx3Aʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQ._ʺx40ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx40ʺTranscriber.Instance.Transcribe(node._ʺx40ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQ._ʺx2Fʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx2FʺTranscriber.Instance.Transcribe(node._ʺx2Fʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQ._ʺx3Fʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx3FʺTranscriber.Instance.Transcribe(node._ʺx3Fʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQ._ʺx24ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx24ʺTranscriber.Instance.Transcribe(node._ʺx24ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQ._ʺx27ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx27ʺTranscriber.Instance.Transcribe(node._ʺx27ʺ_1, context);

return default;
            }
        }
    }
    
}
