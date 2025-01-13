namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _subⲻdelimsTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._subⲻdelims>
    {
        private _subⲻdelimsTranscriber()
        {
        }
        
        public static _subⲻdelimsTranscriber Instance { get; } = new _subⲻdelimsTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._subⲻdelims value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._subⲻdelims.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._subⲻdelims._ʺx24ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx24ʺTranscriber.Instance.Transcribe(node._ʺx24ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._subⲻdelims._ʺx26ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx26ʺTranscriber.Instance.Transcribe(node._ʺx26ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._subⲻdelims._ʺx27ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx27ʺTranscriber.Instance.Transcribe(node._ʺx27ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._subⲻdelims._ʺx3Dʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx3DʺTranscriber.Instance.Transcribe(node._ʺx3Dʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._subⲻdelims._otherⲻdelims node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._otherⲻdelimsTranscriber.Instance.Transcribe(node._otherⲻdelims_1, context);

return default;
            }
        }
    }
    
}
