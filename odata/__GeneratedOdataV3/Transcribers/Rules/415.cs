namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _qcharⲻunescapedTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._qcharⲻunescaped>
    {
        private _qcharⲻunescapedTranscriber()
        {
        }
        
        public static _qcharⲻunescapedTranscriber Instance { get; } = new _qcharⲻunescapedTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._qcharⲻunescaped value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Rules._qcharⲻunescaped.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._qcharⲻunescaped._unreserved node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._unreservedTranscriber.Instance.Transcribe(node._unreserved_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._qcharⲻunescaped._pctⲻencodedⲻunescaped node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._pctⲻencodedⲻunescapedTranscriber.Instance.Transcribe(node._pctⲻencodedⲻunescaped_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._qcharⲻunescaped._otherⲻdelims node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._otherⲻdelimsTranscriber.Instance.Transcribe(node._otherⲻdelims_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._qcharⲻunescaped._ʺx3Aʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx3AʺTranscriber.Instance.Transcribe(node._ʺx3Aʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._qcharⲻunescaped._ʺx40ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx40ʺTranscriber.Instance.Transcribe(node._ʺx40ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._qcharⲻunescaped._ʺx2Fʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx2FʺTranscriber.Instance.Transcribe(node._ʺx2Fʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._qcharⲻunescaped._ʺx3Fʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx3FʺTranscriber.Instance.Transcribe(node._ʺx3Fʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._qcharⲻunescaped._ʺx24ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx24ʺTranscriber.Instance.Transcribe(node._ʺx24ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._qcharⲻunescaped._ʺx27ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx27ʺTranscriber.Instance.Transcribe(node._ʺx27ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._qcharⲻunescaped._ʺx3Dʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx3DʺTranscriber.Instance.Transcribe(node._ʺx3Dʺ_1, context);

return default;
            }
        }
    }
    
}
