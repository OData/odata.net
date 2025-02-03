namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _odataRelativeUriTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._odataRelativeUri>
    {
        private _odataRelativeUriTranscriber()
        {
        }
        
        public static _odataRelativeUriTranscriber Instance { get; } = new _odataRelativeUriTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._odataRelativeUri value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Rules._odataRelativeUri.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x62x61x74x63x68ʺ_꘡ʺx3Fʺ_batchOptions꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx24x62x61x74x63x68ʺTranscriber.Instance.Transcribe(node._ʺx24x62x61x74x63x68ʺ_1, context);
if (node._ʺx3Fʺ_batchOptions_1 != null)
{
__GeneratedOdataV3.Trancsribers.Inners._ʺx3Fʺ_batchOptionsTranscriber.Instance.Transcribe(node._ʺx3Fʺ_batchOptions_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx3Fʺ_entityOptions node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx24x65x6Ex74x69x74x79ʺTranscriber.Instance.Transcribe(node._ʺx24x65x6Ex74x69x74x79ʺ_1, context);
__GeneratedOdataV3.Trancsribers.Inners._ʺx3FʺTranscriber.Instance.Transcribe(node._ʺx3Fʺ_1, context);
__GeneratedOdataV3.Trancsribers.Rules._entityOptionsTranscriber.Instance.Transcribe(node._entityOptions_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptions node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx24x65x6Ex74x69x74x79ʺTranscriber.Instance.Transcribe(node._ʺx24x65x6Ex74x69x74x79ʺ_1, context);
__GeneratedOdataV3.Trancsribers.Inners._ʺx2FʺTranscriber.Instance.Transcribe(node._ʺx2Fʺ_1, context);
__GeneratedOdataV3.Trancsribers.Rules._qualifiedEntityTypeNameTranscriber.Instance.Transcribe(node._qualifiedEntityTypeName_1, context);
__GeneratedOdataV3.Trancsribers.Inners._ʺx3FʺTranscriber.Instance.Transcribe(node._ʺx3Fʺ_1, context);
__GeneratedOdataV3.Trancsribers.Rules._entityCastOptionsTranscriber.Instance.Transcribe(node._entityCastOptions_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x6Dx65x74x61x64x61x74x61ʺ_꘡ʺx3Fʺ_metadataOptions꘡_꘡context꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx24x6Dx65x74x61x64x61x74x61ʺTranscriber.Instance.Transcribe(node._ʺx24x6Dx65x74x61x64x61x74x61ʺ_1, context);
if (node._ʺx3Fʺ_metadataOptions_1 != null)
{
__GeneratedOdataV3.Trancsribers.Inners._ʺx3Fʺ_metadataOptionsTranscriber.Instance.Transcribe(node._ʺx3Fʺ_metadataOptions_1, context);
}
if (node._context_1 != null)
{
__GeneratedOdataV3.Trancsribers.Rules._contextTranscriber.Instance.Transcribe(node._context_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._resourcePath_꘡ʺx3Fʺ_queryOptions꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._resourcePathTranscriber.Instance.Transcribe(node._resourcePath_1, context);
if (node._ʺx3Fʺ_queryOptions_1 != null)
{
__GeneratedOdataV3.Trancsribers.Inners._ʺx3Fʺ_queryOptionsTranscriber.Instance.Transcribe(node._ʺx3Fʺ_queryOptions_1, context);
}

return default;
            }
        }
    }
    
}
