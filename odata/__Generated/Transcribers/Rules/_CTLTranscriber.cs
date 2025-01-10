namespace __Generated.Trancsribers.Rules
{
    public sealed class _CTLTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Rules._CTL>
    {
        private _CTLTranscriber()
        {
        }
        
        public static _CTLTranscriber Instance { get; } = new _CTLTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Rules._CTL value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __Generated.CstNodes.Rules._CTL.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._CTL._Ⰳx00ⲻ1F node, System.Text.StringBuilder context)
            {
                
return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._CTL._Ⰳx7F node, System.Text.StringBuilder context)
            {
                
return default;
            }
        }
    }
    
}
