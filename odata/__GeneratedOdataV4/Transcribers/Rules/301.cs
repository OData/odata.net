namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _hourTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._hour>
    {
        private _hourTranscriber()
        {
        }
        
        public static _hourTranscriber Instance { get; } = new _hourTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._hour value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Rules._hour.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._hour._Ⲥʺx30ʺⳆʺx31ʺↃ_DIGIT node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._Ⲥʺx30ʺⳆʺx31ʺↃTranscriber.Instance.Transcribe(node._Ⲥʺx30ʺⳆʺx31ʺↃ_1, context);
__GeneratedOdataV4.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(node._DIGIT_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._hour._ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx32ʺTranscriber.Instance.Transcribe(node._ʺx32ʺ_1, context);
__GeneratedOdataV4.Trancsribers.Inners._Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃTranscriber.Instance.Transcribe(node._Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃ_1, context);

return default;
            }
        }
    }
    
}
