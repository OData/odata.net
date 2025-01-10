namespace __Generated.Trancsribers.Rules
{
    public sealed class _WSPTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Rules._WSP>
    {
        private _WSPTranscriber()
        {
        }
        
        public static _WSPTranscriber Instance { get; } = new _WSPTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Rules._WSP value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __Generated.CstNodes.Rules._WSP.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._WSP._SP node, System.Text.StringBuilder context)
            {
                
return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._WSP._HTAB node, System.Text.StringBuilder context)
            {
                
return default;
            }
        }
    }
    
}
