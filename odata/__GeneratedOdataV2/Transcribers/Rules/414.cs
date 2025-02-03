namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLARTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR>
    {
        private _qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLARTranscriber()
        {
        }
        
        public static _qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLARTranscriber Instance { get; } = new _qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLARTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._unreserved node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._unreservedTranscriber.Instance.Transcribe(node._unreserved_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._pctⲻencoded node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._pctⲻencodedTranscriber.Instance.Transcribe(node._pctⲻencoded_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._otherⲻdelims node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._otherⲻdelimsTranscriber.Instance.Transcribe(node._otherⲻdelims_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx3Aʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Inners._ʺx3AʺTranscriber.Instance.Transcribe(node._ʺx3Aʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx2Fʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Inners._ʺx2FʺTranscriber.Instance.Transcribe(node._ʺx2Fʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx3Fʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Inners._ʺx3FʺTranscriber.Instance.Transcribe(node._ʺx3Fʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx27ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Inners._ʺx27ʺTranscriber.Instance.Transcribe(node._ʺx27ʺ_1, context);

return default;
            }
        }
    }
    
}
