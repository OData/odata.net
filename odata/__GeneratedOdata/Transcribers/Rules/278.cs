namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _booleanValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._booleanValue>
    {
        private _booleanValueTranscriber()
        {
        }
        
        public static _booleanValueTranscriber Instance { get; } = new _booleanValueTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._booleanValue value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._booleanValue.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._booleanValue._ʺx74x72x75x65ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx74x72x75x65ʺTranscriber.Instance.Transcribe(node._ʺx74x72x75x65ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._booleanValue._ʺx66x61x6Cx73x65ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx66x61x6Cx73x65ʺTranscriber.Instance.Transcribe(node._ʺx66x61x6Cx73x65ʺ_1, context);

return default;
            }
        }
    }
    
}
