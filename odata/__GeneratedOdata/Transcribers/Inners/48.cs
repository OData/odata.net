namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _ʺx2Fʺ_propertyPathⳆboundOperationTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperation>
    {
        private _ʺx2Fʺ_propertyPathⳆboundOperationTranscriber()
        {
        }
        
        public static _ʺx2Fʺ_propertyPathⳆboundOperationTranscriber Instance { get; } = new _ʺx2Fʺ_propertyPathⳆboundOperationTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperation value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperation.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperation._ʺx2Fʺ_propertyPath node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx2FʺTranscriber.Instance.Transcribe(node._ʺx2Fʺ_1, context);
__GeneratedOdata.Trancsribers.Rules._propertyPathTranscriber.Instance.Transcribe(node._propertyPath_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperation._boundOperation node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._boundOperationTranscriber.Instance.Transcribe(node._boundOperation_1, context);

return default;
            }
        }
    }
    
}
