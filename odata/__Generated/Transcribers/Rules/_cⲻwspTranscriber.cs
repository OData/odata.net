namespace __Generated.Trancsribers.Rules
{
    public sealed class _cⲻwspTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Rules._cⲻwsp>
    {
        private _cⲻwspTranscriber()
        {
        }
        
        public static _cⲻwspTranscriber Instance { get; } = new _cⲻwspTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Rules._cⲻwsp value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __Generated.CstNodes.Rules._cⲻwsp.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._cⲻwsp._WSP node, System.Text.StringBuilder context)
            {
                
return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._cⲻwsp._Ⲥcⲻnl_WSPↃ node, System.Text.StringBuilder context)
            {
                
return default;
            }
        }
    }
    
}
