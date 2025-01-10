namespace __Generated.Trancsribers.Rules
{
    public sealed class _HEXDIGTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Rules._HEXDIG>
    {
        private _HEXDIGTranscriber()
        {
        }
        
        public static _HEXDIGTranscriber Instance { get; } = new _HEXDIGTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Rules._HEXDIG value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __Generated.CstNodes.Rules._HEXDIG.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._HEXDIG._DIGIT node, System.Text.StringBuilder context)
            {
                
return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._HEXDIG._ʺx41ʺ node, System.Text.StringBuilder context)
            {
                
return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._HEXDIG._ʺx42ʺ node, System.Text.StringBuilder context)
            {
                
return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._HEXDIG._ʺx43ʺ node, System.Text.StringBuilder context)
            {
                
return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._HEXDIG._ʺx44ʺ node, System.Text.StringBuilder context)
            {
                
return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._HEXDIG._ʺx45ʺ node, System.Text.StringBuilder context)
            {
                
return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._HEXDIG._ʺx46ʺ node, System.Text.StringBuilder context)
            {
                
return default;
            }
        }
    }
    
}
