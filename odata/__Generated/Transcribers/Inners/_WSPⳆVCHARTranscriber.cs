namespace __Generated.Trancsribers.Inners
{
    public sealed class _WSPⳆVCHARTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Inners._WSPⳆVCHAR>
    {
        private _WSPⳆVCHARTranscriber()
        {
        }
        
        public static _WSPⳆVCHARTranscriber Instance { get; } = new _WSPⳆVCHARTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Inners._WSPⳆVCHAR value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __Generated.CstNodes.Inners._WSPⳆVCHAR.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Inners._WSPⳆVCHAR._WSP node, System.Text.StringBuilder context)
            {
                
return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Inners._WSPⳆVCHAR._VCHAR node, System.Text.StringBuilder context)
            {
                
return default;
            }
        }
    }
    
}
