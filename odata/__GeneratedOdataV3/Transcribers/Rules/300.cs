namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _dayTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._day>
    {
        private _dayTranscriber()
        {
        }
        
        public static _dayTranscriber Instance { get; } = new _dayTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._day value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Rules._day.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._day._ʺx30ʺ_oneToNine node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx30ʺTranscriber.Instance.Transcribe(node._ʺx30ʺ_1, context);
__GeneratedOdataV3.Trancsribers.Rules._oneToNineTranscriber.Instance.Transcribe(node._oneToNine_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._day._Ⲥʺx31ʺⳆʺx32ʺↃ_DIGIT node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._Ⲥʺx31ʺⳆʺx32ʺↃTranscriber.Instance.Transcribe(node._Ⲥʺx31ʺⳆʺx32ʺↃ_1, context);
__GeneratedOdataV3.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(node._DIGIT_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._day._ʺx33ʺ_Ⲥʺx30ʺⳆʺx31ʺↃ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx33ʺTranscriber.Instance.Transcribe(node._ʺx33ʺ_1, context);
__GeneratedOdataV3.Trancsribers.Inners._Ⲥʺx30ʺⳆʺx31ʺↃTranscriber.Instance.Transcribe(node._Ⲥʺx30ʺⳆʺx31ʺↃ_1, context);

return default;
            }
        }
    }
    
}
