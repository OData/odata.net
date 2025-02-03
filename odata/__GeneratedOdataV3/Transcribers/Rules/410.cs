namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _pcharⲻnoⲻSQUOTETranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE>
    {
        private _pcharⲻnoⲻSQUOTETranscriber()
        {
        }
        
        public static _pcharⲻnoⲻSQUOTETranscriber Instance { get; } = new _pcharⲻnoⲻSQUOTETranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._unreserved node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._unreservedTranscriber.Instance.Transcribe(node._unreserved_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._pctⲻencodedⲻnoⲻSQUOTE node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._pctⲻencodedⲻnoⲻSQUOTETranscriber.Instance.Transcribe(node._pctⲻencodedⲻnoⲻSQUOTE_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._otherⲻdelims node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._otherⲻdelimsTranscriber.Instance.Transcribe(node._otherⲻdelims_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx24ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx24ʺTranscriber.Instance.Transcribe(node._ʺx24ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx26ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx26ʺTranscriber.Instance.Transcribe(node._ʺx26ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx3Dʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx3DʺTranscriber.Instance.Transcribe(node._ʺx3Dʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx3Aʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx3AʺTranscriber.Instance.Transcribe(node._ʺx3Aʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx40ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx40ʺTranscriber.Instance.Transcribe(node._ʺx40ʺ_1, context);

return default;
            }
        }
    }
    
}
