namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue>
    {
        private _ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalueTranscriber()
        {
        }
        
        public static _ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalueTranscriber Instance { get; } = new _ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalueTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue._ʺx2Fʺ_propertyPath node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx2FʺTranscriber.Instance.Transcribe(node._ʺx2Fʺ_1, context);
__GeneratedOdataV4.Trancsribers.Rules._propertyPathTranscriber.Instance.Transcribe(node._propertyPath_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue._boundOperation node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._boundOperationTranscriber.Instance.Transcribe(node._boundOperation_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue._ref node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._refTranscriber.Instance.Transcribe(node._ref_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue._value node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._valueTranscriber.Instance.Transcribe(node._value_1, context);

return default;
            }
        }
    }
    
}
