namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _quotationⲻmarkTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._quotationⲻmark>
    {
        private _quotationⲻmarkTranscriber()
        {
        }
        
        public static _quotationⲻmarkTranscriber Instance { get; } = new _quotationⲻmarkTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._quotationⲻmark value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Rules._quotationⲻmark.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._quotationⲻmark._DQUOTE node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._DQUOTETranscriber.Instance.Transcribe(node._DQUOTE_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._quotationⲻmark._ʺx25x32x32ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx25x32x32ʺTranscriber.Instance.Transcribe(node._ʺx25x32x32ʺ_1, context);

return default;
            }
        }
    }
    
}
