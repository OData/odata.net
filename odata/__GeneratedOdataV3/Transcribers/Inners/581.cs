namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _termNameⳆSTARTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._termNameⳆSTAR>
    {
        private _termNameⳆSTARTranscriber()
        {
        }
        
        public static _termNameⳆSTARTranscriber Instance { get; } = new _termNameⳆSTARTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._termNameⳆSTAR value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Inners._termNameⳆSTAR.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._termNameⳆSTAR._termName node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._termNameTranscriber.Instance.Transcribe(node._termName_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._termNameⳆSTAR._STAR node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._STARTranscriber.Instance.Transcribe(node._STAR_1, context);

return default;
            }
        }
    }
    
}
