namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _qcharⲻnoⲻAMPTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP>
    {
        private _qcharⲻnoⲻAMPTranscriber()
        {
        }
        
        public static _qcharⲻnoⲻAMPTranscriber Instance { get; } = new _qcharⲻnoⲻAMPTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._unreserved node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._unreservedTranscriber.Instance.Transcribe(node._unreserved_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._pctⲻencoded node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._pctⲻencodedTranscriber.Instance.Transcribe(node._pctⲻencoded_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._otherⲻdelims node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._otherⲻdelimsTranscriber.Instance.Transcribe(node._otherⲻdelims_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx3Aʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx3AʺTranscriber.Instance.Transcribe(node._ʺx3Aʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx40ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx40ʺTranscriber.Instance.Transcribe(node._ʺx40ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx2Fʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx2FʺTranscriber.Instance.Transcribe(node._ʺx2Fʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx3Fʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx3FʺTranscriber.Instance.Transcribe(node._ʺx3Fʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx24ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx24ʺTranscriber.Instance.Transcribe(node._ʺx24ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx27ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx27ʺTranscriber.Instance.Transcribe(node._ʺx27ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx3Dʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx3DʺTranscriber.Instance.Transcribe(node._ʺx3Dʺ_1, context);

return default;
            }
        }
    }
    
}
