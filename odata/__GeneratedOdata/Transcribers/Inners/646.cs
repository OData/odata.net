namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _IPv6addressⳆIPvFutureTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._IPv6addressⳆIPvFuture>
    {
        private _IPv6addressⳆIPvFutureTranscriber()
        {
        }
        
        public static _IPv6addressⳆIPvFutureTranscriber Instance { get; } = new _IPv6addressⳆIPvFutureTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._IPv6addressⳆIPvFuture value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._IPv6addressⳆIPvFuture.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._IPv6addressⳆIPvFuture._IPv6address node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._IPv6addressTranscriber.Instance.Transcribe(node._IPv6address_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._IPv6addressⳆIPvFuture._IPvFuture node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._IPvFutureTranscriber.Instance.Transcribe(node._IPvFuture_1, context);

return default;
            }
        }
    }
    
}
