namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _countⳆboundOperationTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._countⳆboundOperation>
    {
        private _countⳆboundOperationTranscriber()
        {
        }
        
        public static _countⳆboundOperationTranscriber Instance { get; } = new _countⳆboundOperationTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._countⳆboundOperation value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Inners._countⳆboundOperation.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._countⳆboundOperation._count node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._countTranscriber.Instance.Transcribe(node._count_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._countⳆboundOperation._boundOperation node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._boundOperationTranscriber.Instance.Transcribe(node._boundOperation_1, context);

return default;
            }
        }
    }
    
}
