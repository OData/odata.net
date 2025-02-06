namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _entityColNavigationProperty_꘡collectionNavigationExpr꘡ⳆentityNavigationProperty_꘡singleNavigationExpr꘡ⳆcomplexColProperty_꘡complexColPathExpr꘡ⳆcomplexProperty_꘡complexPathExpr꘡ⳆprimitiveColProperty_꘡collectionPathExpr꘡ⳆprimitiveProperty_꘡primitivePathExpr꘡ⳆstreamProperty_꘡primitivePathExpr꘡Transcriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._entityColNavigationProperty_꘡collectionNavigationExpr꘡ⳆentityNavigationProperty_꘡singleNavigationExpr꘡ⳆcomplexColProperty_꘡complexColPathExpr꘡ⳆcomplexProperty_꘡complexPathExpr꘡ⳆprimitiveColProperty_꘡collectionPathExpr꘡ⳆprimitiveProperty_꘡primitivePathExpr꘡ⳆstreamProperty_꘡primitivePathExpr꘡>
    {
        private _entityColNavigationProperty_꘡collectionNavigationExpr꘡ⳆentityNavigationProperty_꘡singleNavigationExpr꘡ⳆcomplexColProperty_꘡complexColPathExpr꘡ⳆcomplexProperty_꘡complexPathExpr꘡ⳆprimitiveColProperty_꘡collectionPathExpr꘡ⳆprimitiveProperty_꘡primitivePathExpr꘡ⳆstreamProperty_꘡primitivePathExpr꘡Transcriber()
        {
        }
        
        public static _entityColNavigationProperty_꘡collectionNavigationExpr꘡ⳆentityNavigationProperty_꘡singleNavigationExpr꘡ⳆcomplexColProperty_꘡complexColPathExpr꘡ⳆcomplexProperty_꘡complexPathExpr꘡ⳆprimitiveColProperty_꘡collectionPathExpr꘡ⳆprimitiveProperty_꘡primitivePathExpr꘡ⳆstreamProperty_꘡primitivePathExpr꘡Transcriber Instance { get; } = new _entityColNavigationProperty_꘡collectionNavigationExpr꘡ⳆentityNavigationProperty_꘡singleNavigationExpr꘡ⳆcomplexColProperty_꘡complexColPathExpr꘡ⳆcomplexProperty_꘡complexPathExpr꘡ⳆprimitiveColProperty_꘡collectionPathExpr꘡ⳆprimitiveProperty_꘡primitivePathExpr꘡ⳆstreamProperty_꘡primitivePathExpr꘡Transcriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._entityColNavigationProperty_꘡collectionNavigationExpr꘡ⳆentityNavigationProperty_꘡singleNavigationExpr꘡ⳆcomplexColProperty_꘡complexColPathExpr꘡ⳆcomplexProperty_꘡complexPathExpr꘡ⳆprimitiveColProperty_꘡collectionPathExpr꘡ⳆprimitiveProperty_꘡primitivePathExpr꘡ⳆstreamProperty_꘡primitivePathExpr꘡ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Inners._entityColNavigationProperty_꘡collectionNavigationExpr꘡ⳆentityNavigationProperty_꘡singleNavigationExpr꘡ⳆcomplexColProperty_꘡complexColPathExpr꘡ⳆcomplexProperty_꘡complexPathExpr꘡ⳆprimitiveColProperty_꘡collectionPathExpr꘡ⳆprimitiveProperty_꘡primitivePathExpr꘡ⳆstreamProperty_꘡primitivePathExpr꘡.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._entityColNavigationProperty_꘡collectionNavigationExpr꘡ⳆentityNavigationProperty_꘡singleNavigationExpr꘡ⳆcomplexColProperty_꘡complexColPathExpr꘡ⳆcomplexProperty_꘡complexPathExpr꘡ⳆprimitiveColProperty_꘡collectionPathExpr꘡ⳆprimitiveProperty_꘡primitivePathExpr꘡ⳆstreamProperty_꘡primitivePathExpr꘡._entityColNavigationProperty_꘡collectionNavigationExpr꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._entityColNavigationPropertyTranscriber.Instance.Transcribe(node._entityColNavigationProperty_1, context);
if (node._collectionNavigationExpr_1 != null)
{
__GeneratedOdataV4.Trancsribers.Rules._collectionNavigationExprTranscriber.Instance.Transcribe(node._collectionNavigationExpr_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._entityColNavigationProperty_꘡collectionNavigationExpr꘡ⳆentityNavigationProperty_꘡singleNavigationExpr꘡ⳆcomplexColProperty_꘡complexColPathExpr꘡ⳆcomplexProperty_꘡complexPathExpr꘡ⳆprimitiveColProperty_꘡collectionPathExpr꘡ⳆprimitiveProperty_꘡primitivePathExpr꘡ⳆstreamProperty_꘡primitivePathExpr꘡._entityNavigationProperty_꘡singleNavigationExpr꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._entityNavigationPropertyTranscriber.Instance.Transcribe(node._entityNavigationProperty_1, context);
if (node._singleNavigationExpr_1 != null)
{
__GeneratedOdataV4.Trancsribers.Rules._singleNavigationExprTranscriber.Instance.Transcribe(node._singleNavigationExpr_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._entityColNavigationProperty_꘡collectionNavigationExpr꘡ⳆentityNavigationProperty_꘡singleNavigationExpr꘡ⳆcomplexColProperty_꘡complexColPathExpr꘡ⳆcomplexProperty_꘡complexPathExpr꘡ⳆprimitiveColProperty_꘡collectionPathExpr꘡ⳆprimitiveProperty_꘡primitivePathExpr꘡ⳆstreamProperty_꘡primitivePathExpr꘡._complexColProperty_꘡complexColPathExpr꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._complexColPropertyTranscriber.Instance.Transcribe(node._complexColProperty_1, context);
if (node._complexColPathExpr_1 != null)
{
__GeneratedOdataV4.Trancsribers.Rules._complexColPathExprTranscriber.Instance.Transcribe(node._complexColPathExpr_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._entityColNavigationProperty_꘡collectionNavigationExpr꘡ⳆentityNavigationProperty_꘡singleNavigationExpr꘡ⳆcomplexColProperty_꘡complexColPathExpr꘡ⳆcomplexProperty_꘡complexPathExpr꘡ⳆprimitiveColProperty_꘡collectionPathExpr꘡ⳆprimitiveProperty_꘡primitivePathExpr꘡ⳆstreamProperty_꘡primitivePathExpr꘡._complexProperty_꘡complexPathExpr꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._complexPropertyTranscriber.Instance.Transcribe(node._complexProperty_1, context);
if (node._complexPathExpr_1 != null)
{
__GeneratedOdataV4.Trancsribers.Rules._complexPathExprTranscriber.Instance.Transcribe(node._complexPathExpr_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._entityColNavigationProperty_꘡collectionNavigationExpr꘡ⳆentityNavigationProperty_꘡singleNavigationExpr꘡ⳆcomplexColProperty_꘡complexColPathExpr꘡ⳆcomplexProperty_꘡complexPathExpr꘡ⳆprimitiveColProperty_꘡collectionPathExpr꘡ⳆprimitiveProperty_꘡primitivePathExpr꘡ⳆstreamProperty_꘡primitivePathExpr꘡._primitiveColProperty_꘡collectionPathExpr꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._primitiveColPropertyTranscriber.Instance.Transcribe(node._primitiveColProperty_1, context);
if (node._collectionPathExpr_1 != null)
{
__GeneratedOdataV4.Trancsribers.Rules._collectionPathExprTranscriber.Instance.Transcribe(node._collectionPathExpr_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._entityColNavigationProperty_꘡collectionNavigationExpr꘡ⳆentityNavigationProperty_꘡singleNavigationExpr꘡ⳆcomplexColProperty_꘡complexColPathExpr꘡ⳆcomplexProperty_꘡complexPathExpr꘡ⳆprimitiveColProperty_꘡collectionPathExpr꘡ⳆprimitiveProperty_꘡primitivePathExpr꘡ⳆstreamProperty_꘡primitivePathExpr꘡._primitiveProperty_꘡primitivePathExpr꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._primitivePropertyTranscriber.Instance.Transcribe(node._primitiveProperty_1, context);
if (node._primitivePathExpr_1 != null)
{
__GeneratedOdataV4.Trancsribers.Rules._primitivePathExprTranscriber.Instance.Transcribe(node._primitivePathExpr_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._entityColNavigationProperty_꘡collectionNavigationExpr꘡ⳆentityNavigationProperty_꘡singleNavigationExpr꘡ⳆcomplexColProperty_꘡complexColPathExpr꘡ⳆcomplexProperty_꘡complexPathExpr꘡ⳆprimitiveColProperty_꘡collectionPathExpr꘡ⳆprimitiveProperty_꘡primitivePathExpr꘡ⳆstreamProperty_꘡primitivePathExpr꘡._streamProperty_꘡primitivePathExpr꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._streamPropertyTranscriber.Instance.Transcribe(node._streamProperty_1, context);
if (node._primitivePathExpr_1 != null)
{
__GeneratedOdataV4.Trancsribers.Rules._primitivePathExprTranscriber.Instance.Transcribe(node._primitivePathExpr_1, context);
}

return default;
            }
        }
    }
    
}
