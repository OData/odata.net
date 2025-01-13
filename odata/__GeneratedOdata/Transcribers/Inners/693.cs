namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _VCHARⳆobsⲻtextTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._VCHARⳆobsⲻtext>
    {
        private _VCHARⳆobsⲻtextTranscriber()
        {
        }
        
        public static _VCHARⳆobsⲻtextTranscriber Instance { get; } = new _VCHARⳆobsⲻtextTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._VCHARⳆobsⲻtext value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._VCHARⳆobsⲻtext.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._VCHARⳆobsⲻtext._VCHAR node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._VCHARTranscriber.Instance.Transcribe(node._VCHAR_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._VCHARⳆobsⲻtext._obsⲻtext node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._obsⲻtextTranscriber.Instance.Transcribe(node._obsⲻtext_1, context);

return default;
            }
        }
    }
    
}
