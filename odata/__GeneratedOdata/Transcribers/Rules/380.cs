namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _SQUOTETranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._SQUOTE>
    {
        private _SQUOTETranscriber()
        {
        }
        
        public static _SQUOTETranscriber Instance { get; } = new _SQUOTETranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._SQUOTE value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._SQUOTE.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._SQUOTE._ʺx27ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx27ʺTranscriber.Instance.Transcribe(node._ʺx27ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._SQUOTE._ʺx25x32x37ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx25x32x37ʺTranscriber.Instance.Transcribe(node._ʺx25x32x37ʺ_1, context);

return default;
            }
        }
    }
    
}
