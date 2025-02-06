namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Transcriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡>
    {
        private _STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Transcriber()
        {
        }
        
        public static _STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Transcriber Instance { get; } = new _STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Transcriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Inners._STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡._STAR node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._STARTranscriber.Instance.Transcribe(node._STAR_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡._streamProperty node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._streamPropertyTranscriber.Instance.Transcribe(node._streamProperty_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡._navigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._navigationPropertyTranscriber.Instance.Transcribe(node._navigationProperty_1, context);
if (node._ʺx2Fʺ_qualifiedEntityTypeName_1 != null)
{
__GeneratedOdataV4.Trancsribers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameTranscriber.Instance.Transcribe(node._ʺx2Fʺ_qualifiedEntityTypeName_1, context);
}

return default;
            }
        }
    }
    
}
