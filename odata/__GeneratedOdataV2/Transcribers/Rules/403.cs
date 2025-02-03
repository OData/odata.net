namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _pcharTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._pchar>
    {
        private _pcharTranscriber()
        {
        }
        
        public static _pcharTranscriber Instance { get; } = new _pcharTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._pchar value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV2.CstNodes.Rules._pchar.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._pchar._unreserved node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._unreservedTranscriber.Instance.Transcribe(node._unreserved_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._pchar._pctⲻencoded node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._pctⲻencodedTranscriber.Instance.Transcribe(node._pctⲻencoded_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._pchar._subⲻdelims node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._subⲻdelimsTranscriber.Instance.Transcribe(node._subⲻdelims_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._pchar._ʺx3Aʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Inners._ʺx3AʺTranscriber.Instance.Transcribe(node._ʺx3Aʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._pchar._ʺx40ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Inners._ʺx40ʺTranscriber.Instance.Transcribe(node._ʺx40ʺ_1, context);

return default;
            }
        }
    }
    
}
