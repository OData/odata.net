namespace __Generated.Trancsribers.Rules
{
    public sealed class _elementTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Rules._element>
    {
        private _elementTranscriber()
        {
        }
        
        public static _elementTranscriber Instance { get; } = new _elementTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Rules._element value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __Generated.CstNodes.Rules._element.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._element._rulename node, System.Text.StringBuilder context)
            {
                
return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._element._group node, System.Text.StringBuilder context)
            {
                
return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._element._option node, System.Text.StringBuilder context)
            {
                
return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._element._charⲻval node, System.Text.StringBuilder context)
            {
                
return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._element._numⲻval node, System.Text.StringBuilder context)
            {
                
return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._element._proseⲻval node, System.Text.StringBuilder context)
            {
                
return default;
            }
        }
    }
    
}
