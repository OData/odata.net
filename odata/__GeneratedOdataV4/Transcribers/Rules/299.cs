namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _monthTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._month>
    {
        private _monthTranscriber()
        {
        }
        
        public static _monthTranscriber Instance { get; } = new _monthTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._month value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Rules._month.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._month._ʺx30ʺ_oneToNine node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx30ʺTranscriber.Instance.Transcribe(node._ʺx30ʺ_1, context);
__GeneratedOdataV4.Trancsribers.Rules._oneToNineTranscriber.Instance.Transcribe(node._oneToNine_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._month._ʺx31ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺↃ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx31ʺTranscriber.Instance.Transcribe(node._ʺx31ʺ_1, context);
__GeneratedOdataV4.Trancsribers.Inners._Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺↃTranscriber.Instance.Transcribe(node._Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺↃ_1, context);

return default;
            }
        }
    }
    
}
