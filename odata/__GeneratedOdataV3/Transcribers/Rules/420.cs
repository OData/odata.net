namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _ALPHATranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._ALPHA>
    {
        private _ALPHATranscriber()
        {
        }
        
        public static _ALPHATranscriber Instance { get; } = new _ALPHATranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._ALPHA value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Rules._ALPHA.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._ALPHA._Ⰳx41ⲻ5A node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._Ⰳx41ⲻ5ATranscriber.Instance.Transcribe(node._Ⰳx41ⲻ5A_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._Ⰳx61ⲻ7ATranscriber.Instance.Transcribe(node._Ⰳx61ⲻ7A_1, context);

return default;
            }
        }
    }
    
}
