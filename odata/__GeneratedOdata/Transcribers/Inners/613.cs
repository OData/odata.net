namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _SPⳆHTABTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._SPⳆHTAB>
    {
        private _SPⳆHTABTranscriber()
        {
        }
        
        public static _SPⳆHTABTranscriber Instance { get; } = new _SPⳆHTABTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._SPⳆHTAB value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._SPⳆHTAB.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._SPⳆHTAB._SP node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._SPTranscriber.Instance.Transcribe(node._SP_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._SPⳆHTAB._HTAB node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._HTABTranscriber.Instance.Transcribe(node._HTAB_1, context);

return default;
            }
        }
    }
    
}
