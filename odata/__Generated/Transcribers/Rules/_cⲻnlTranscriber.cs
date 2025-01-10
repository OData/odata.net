namespace __Generated.Trancsribers.Rules
{
    public sealed class _cⲻnlTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Rules._cⲻnl>
    {
        private _cⲻnlTranscriber()
        {
        }
        
        public static _cⲻnlTranscriber Instance { get; } = new _cⲻnlTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Rules._cⲻnl value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __Generated.CstNodes.Rules._cⲻnl.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._cⲻnl._comment node, System.Text.StringBuilder context)
            {
                __Generated.Trancsribers.Rules._commentTranscriber.Instance.Transcribe(node._comment_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._cⲻnl._CRLF node, System.Text.StringBuilder context)
            {
                __Generated.Trancsribers.Rules._CRLFTranscriber.Instance.Transcribe(node._CRLF_1, context);

return default;
            }
        }
    }
    
}
