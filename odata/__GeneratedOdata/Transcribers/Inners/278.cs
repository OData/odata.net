namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _entityColFunction_functionExprParameters_꘡collectionNavigationExpr꘡ⳆentityFunction_functionExprParameters_꘡singleNavigationExpr꘡ⳆcomplexColFunction_functionExprParameters_꘡complexColPathExpr꘡ⳆcomplexFunction_functionExprParameters_꘡complexPathExpr꘡ⳆprimitiveColFunction_functionExprParameters_꘡collectionPathExpr꘡ⳆprimitiveFunction_functionExprParameters_꘡primitivePathExpr꘡Transcriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._entityColFunction_functionExprParameters_꘡collectionNavigationExpr꘡ⳆentityFunction_functionExprParameters_꘡singleNavigationExpr꘡ⳆcomplexColFunction_functionExprParameters_꘡complexColPathExpr꘡ⳆcomplexFunction_functionExprParameters_꘡complexPathExpr꘡ⳆprimitiveColFunction_functionExprParameters_꘡collectionPathExpr꘡ⳆprimitiveFunction_functionExprParameters_꘡primitivePathExpr꘡>
    {
        private _entityColFunction_functionExprParameters_꘡collectionNavigationExpr꘡ⳆentityFunction_functionExprParameters_꘡singleNavigationExpr꘡ⳆcomplexColFunction_functionExprParameters_꘡complexColPathExpr꘡ⳆcomplexFunction_functionExprParameters_꘡complexPathExpr꘡ⳆprimitiveColFunction_functionExprParameters_꘡collectionPathExpr꘡ⳆprimitiveFunction_functionExprParameters_꘡primitivePathExpr꘡Transcriber()
        {
        }
        
        public static _entityColFunction_functionExprParameters_꘡collectionNavigationExpr꘡ⳆentityFunction_functionExprParameters_꘡singleNavigationExpr꘡ⳆcomplexColFunction_functionExprParameters_꘡complexColPathExpr꘡ⳆcomplexFunction_functionExprParameters_꘡complexPathExpr꘡ⳆprimitiveColFunction_functionExprParameters_꘡collectionPathExpr꘡ⳆprimitiveFunction_functionExprParameters_꘡primitivePathExpr꘡Transcriber Instance { get; } = new _entityColFunction_functionExprParameters_꘡collectionNavigationExpr꘡ⳆentityFunction_functionExprParameters_꘡singleNavigationExpr꘡ⳆcomplexColFunction_functionExprParameters_꘡complexColPathExpr꘡ⳆcomplexFunction_functionExprParameters_꘡complexPathExpr꘡ⳆprimitiveColFunction_functionExprParameters_꘡collectionPathExpr꘡ⳆprimitiveFunction_functionExprParameters_꘡primitivePathExpr꘡Transcriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._entityColFunction_functionExprParameters_꘡collectionNavigationExpr꘡ⳆentityFunction_functionExprParameters_꘡singleNavigationExpr꘡ⳆcomplexColFunction_functionExprParameters_꘡complexColPathExpr꘡ⳆcomplexFunction_functionExprParameters_꘡complexPathExpr꘡ⳆprimitiveColFunction_functionExprParameters_꘡collectionPathExpr꘡ⳆprimitiveFunction_functionExprParameters_꘡primitivePathExpr꘡ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._entityColFunction_functionExprParameters_꘡collectionNavigationExpr꘡ⳆentityFunction_functionExprParameters_꘡singleNavigationExpr꘡ⳆcomplexColFunction_functionExprParameters_꘡complexColPathExpr꘡ⳆcomplexFunction_functionExprParameters_꘡complexPathExpr꘡ⳆprimitiveColFunction_functionExprParameters_꘡collectionPathExpr꘡ⳆprimitiveFunction_functionExprParameters_꘡primitivePathExpr꘡.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._entityColFunction_functionExprParameters_꘡collectionNavigationExpr꘡ⳆentityFunction_functionExprParameters_꘡singleNavigationExpr꘡ⳆcomplexColFunction_functionExprParameters_꘡complexColPathExpr꘡ⳆcomplexFunction_functionExprParameters_꘡complexPathExpr꘡ⳆprimitiveColFunction_functionExprParameters_꘡collectionPathExpr꘡ⳆprimitiveFunction_functionExprParameters_꘡primitivePathExpr꘡._entityColFunction_functionExprParameters_꘡collectionNavigationExpr꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._entityColFunctionTranscriber.Instance.Transcribe(node._entityColFunction_1, context);
__GeneratedOdata.Trancsribers.Rules._functionExprParametersTranscriber.Instance.Transcribe(node._functionExprParameters_1, context);
if (node._collectionNavigationExpr_1 != null)
{
__GeneratedOdata.Trancsribers.Rules._collectionNavigationExprTranscriber.Instance.Transcribe(node._collectionNavigationExpr_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._entityColFunction_functionExprParameters_꘡collectionNavigationExpr꘡ⳆentityFunction_functionExprParameters_꘡singleNavigationExpr꘡ⳆcomplexColFunction_functionExprParameters_꘡complexColPathExpr꘡ⳆcomplexFunction_functionExprParameters_꘡complexPathExpr꘡ⳆprimitiveColFunction_functionExprParameters_꘡collectionPathExpr꘡ⳆprimitiveFunction_functionExprParameters_꘡primitivePathExpr꘡._entityFunction_functionExprParameters_꘡singleNavigationExpr꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._entityFunctionTranscriber.Instance.Transcribe(node._entityFunction_1, context);
__GeneratedOdata.Trancsribers.Rules._functionExprParametersTranscriber.Instance.Transcribe(node._functionExprParameters_1, context);
if (node._singleNavigationExpr_1 != null)
{
__GeneratedOdata.Trancsribers.Rules._singleNavigationExprTranscriber.Instance.Transcribe(node._singleNavigationExpr_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._entityColFunction_functionExprParameters_꘡collectionNavigationExpr꘡ⳆentityFunction_functionExprParameters_꘡singleNavigationExpr꘡ⳆcomplexColFunction_functionExprParameters_꘡complexColPathExpr꘡ⳆcomplexFunction_functionExprParameters_꘡complexPathExpr꘡ⳆprimitiveColFunction_functionExprParameters_꘡collectionPathExpr꘡ⳆprimitiveFunction_functionExprParameters_꘡primitivePathExpr꘡._complexColFunction_functionExprParameters_꘡complexColPathExpr꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._complexColFunctionTranscriber.Instance.Transcribe(node._complexColFunction_1, context);
__GeneratedOdata.Trancsribers.Rules._functionExprParametersTranscriber.Instance.Transcribe(node._functionExprParameters_1, context);
if (node._complexColPathExpr_1 != null)
{
__GeneratedOdata.Trancsribers.Rules._complexColPathExprTranscriber.Instance.Transcribe(node._complexColPathExpr_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._entityColFunction_functionExprParameters_꘡collectionNavigationExpr꘡ⳆentityFunction_functionExprParameters_꘡singleNavigationExpr꘡ⳆcomplexColFunction_functionExprParameters_꘡complexColPathExpr꘡ⳆcomplexFunction_functionExprParameters_꘡complexPathExpr꘡ⳆprimitiveColFunction_functionExprParameters_꘡collectionPathExpr꘡ⳆprimitiveFunction_functionExprParameters_꘡primitivePathExpr꘡._complexFunction_functionExprParameters_꘡complexPathExpr꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._complexFunctionTranscriber.Instance.Transcribe(node._complexFunction_1, context);
__GeneratedOdata.Trancsribers.Rules._functionExprParametersTranscriber.Instance.Transcribe(node._functionExprParameters_1, context);
if (node._complexPathExpr_1 != null)
{
__GeneratedOdata.Trancsribers.Rules._complexPathExprTranscriber.Instance.Transcribe(node._complexPathExpr_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._entityColFunction_functionExprParameters_꘡collectionNavigationExpr꘡ⳆentityFunction_functionExprParameters_꘡singleNavigationExpr꘡ⳆcomplexColFunction_functionExprParameters_꘡complexColPathExpr꘡ⳆcomplexFunction_functionExprParameters_꘡complexPathExpr꘡ⳆprimitiveColFunction_functionExprParameters_꘡collectionPathExpr꘡ⳆprimitiveFunction_functionExprParameters_꘡primitivePathExpr꘡._primitiveColFunction_functionExprParameters_꘡collectionPathExpr꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._primitiveColFunctionTranscriber.Instance.Transcribe(node._primitiveColFunction_1, context);
__GeneratedOdata.Trancsribers.Rules._functionExprParametersTranscriber.Instance.Transcribe(node._functionExprParameters_1, context);
if (node._collectionPathExpr_1 != null)
{
__GeneratedOdata.Trancsribers.Rules._collectionPathExprTranscriber.Instance.Transcribe(node._collectionPathExpr_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._entityColFunction_functionExprParameters_꘡collectionNavigationExpr꘡ⳆentityFunction_functionExprParameters_꘡singleNavigationExpr꘡ⳆcomplexColFunction_functionExprParameters_꘡complexColPathExpr꘡ⳆcomplexFunction_functionExprParameters_꘡complexPathExpr꘡ⳆprimitiveColFunction_functionExprParameters_꘡collectionPathExpr꘡ⳆprimitiveFunction_functionExprParameters_꘡primitivePathExpr꘡._primitiveFunction_functionExprParameters_꘡primitivePathExpr꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._primitiveFunctionTranscriber.Instance.Transcribe(node._primitiveFunction_1, context);
__GeneratedOdata.Trancsribers.Rules._functionExprParametersTranscriber.Instance.Transcribe(node._functionExprParameters_1, context);
if (node._primitivePathExpr_1 != null)
{
__GeneratedOdata.Trancsribers.Rules._primitivePathExprTranscriber.Instance.Transcribe(node._primitivePathExpr_1, context);
}

return default;
            }
        }
    }
    
}