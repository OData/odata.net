namespace __Generated.Trancsribers.Rules
{
    public sealed class _repeatTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Rules._repeat>
    {
        private _repeatTranscriber()
        {
        }
        
        public static _repeatTranscriber Instance { get; } = new _repeatTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Rules._repeat value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __Generated.CstNodes.Rules._repeat.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._repeat._1ЖDIGIT node, System.Text.StringBuilder context)
            {
                
return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._repeat._ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ node, System.Text.StringBuilder context)
            {
                
return default;
            }
        }
    }
    
}
