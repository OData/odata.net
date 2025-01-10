namespace __Generated.Trancsribers.Inners
{
    public sealed class _WSPⳆCRLF_WSPTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Inners._WSPⳆCRLF_WSP>
    {
        private _WSPⳆCRLF_WSPTranscriber()
        {
        }
        
        public static _WSPⳆCRLF_WSPTranscriber Instance { get; } = new _WSPⳆCRLF_WSPTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Inners._WSPⳆCRLF_WSP value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __Generated.CstNodes.Inners._WSPⳆCRLF_WSP.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Inners._WSPⳆCRLF_WSP._WSP node, System.Text.StringBuilder context)
            {
                
return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Inners._WSPⳆCRLF_WSP._CRLF_WSP node, System.Text.StringBuilder context)
            {
                
return default;
            }
        }
    }
    
}
