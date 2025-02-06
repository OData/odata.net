namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _ATTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._AT>
    {
        private _ATTranscriber()
        {
        }
        
        public static _ATTranscriber Instance { get; } = new _ATTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._AT value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Rules._AT.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._AT._ʺx40ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx40ʺTranscriber.Instance.Transcribe(node._ʺx40ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._AT._ʺx25x34x30ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx25x34x30ʺTranscriber.Instance.Transcribe(node._ʺx25x34x30ʺ_1, context);

return default;
            }
        }
    }
    
}
