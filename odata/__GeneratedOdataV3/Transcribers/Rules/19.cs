namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _complexColPathTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._complexColPath>
    {
        private _complexColPathTranscriber()
        {
        }
        
        public static _complexColPathTranscriber Instance { get; } = new _complexColPathTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._complexColPath value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Rules._complexColPath.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._complexColPath._ordinalIndex node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._ordinalIndexTranscriber.Instance.Transcribe(node._ordinalIndex_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._complexColPath._꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_꘡countⳆboundOperation꘡ node, System.Text.StringBuilder context)
            {
                if (node._ʺx2Fʺ_qualifiedComplexTypeName_1 != null)
{
__GeneratedOdataV3.Trancsribers.Inners._ʺx2Fʺ_qualifiedComplexTypeNameTranscriber.Instance.Transcribe(node._ʺx2Fʺ_qualifiedComplexTypeName_1, context);
}
if (node._countⳆboundOperation_1 != null)
{
__GeneratedOdataV3.Trancsribers.Inners._countⳆboundOperationTranscriber.Instance.Transcribe(node._countⳆboundOperation_1, context);
}

return default;
            }
        }
    }
    
}
