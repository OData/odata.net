namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _resourcePathTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._resourcePath>
    {
        private _resourcePathTranscriber()
        {
        }
        
        public static _resourcePathTranscriber Instance { get; } = new _resourcePathTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._resourcePath value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Rules._resourcePath.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._resourcePath._entitySetName_꘡collectionNavigation꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._entitySetNameTranscriber.Instance.Transcribe(node._entitySetName_1, context);
if (node._collectionNavigation_1 != null)
{
__GeneratedOdataV3.Trancsribers.Rules._collectionNavigationTranscriber.Instance.Transcribe(node._collectionNavigation_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._resourcePath._singletonEntity_꘡singleNavigation꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._singletonEntityTranscriber.Instance.Transcribe(node._singletonEntity_1, context);
if (node._singleNavigation_1 != null)
{
__GeneratedOdataV3.Trancsribers.Rules._singleNavigationTranscriber.Instance.Transcribe(node._singleNavigation_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._resourcePath._actionImportCall node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._actionImportCallTranscriber.Instance.Transcribe(node._actionImportCall_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._resourcePath._entityColFunctionImportCall_꘡collectionNavigation꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._entityColFunctionImportCallTranscriber.Instance.Transcribe(node._entityColFunctionImportCall_1, context);
if (node._collectionNavigation_1 != null)
{
__GeneratedOdataV3.Trancsribers.Rules._collectionNavigationTranscriber.Instance.Transcribe(node._collectionNavigation_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._resourcePath._entityFunctionImportCall_꘡singleNavigation꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._entityFunctionImportCallTranscriber.Instance.Transcribe(node._entityFunctionImportCall_1, context);
if (node._singleNavigation_1 != null)
{
__GeneratedOdataV3.Trancsribers.Rules._singleNavigationTranscriber.Instance.Transcribe(node._singleNavigation_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._resourcePath._complexColFunctionImportCall_꘡complexColPath꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._complexColFunctionImportCallTranscriber.Instance.Transcribe(node._complexColFunctionImportCall_1, context);
if (node._complexColPath_1 != null)
{
__GeneratedOdataV3.Trancsribers.Rules._complexColPathTranscriber.Instance.Transcribe(node._complexColPath_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._resourcePath._complexFunctionImportCall_꘡complexPath꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._complexFunctionImportCallTranscriber.Instance.Transcribe(node._complexFunctionImportCall_1, context);
if (node._complexPath_1 != null)
{
__GeneratedOdataV3.Trancsribers.Rules._complexPathTranscriber.Instance.Transcribe(node._complexPath_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._resourcePath._primitiveColFunctionImportCall_꘡primitiveColPath꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._primitiveColFunctionImportCallTranscriber.Instance.Transcribe(node._primitiveColFunctionImportCall_1, context);
if (node._primitiveColPath_1 != null)
{
__GeneratedOdataV3.Trancsribers.Rules._primitiveColPathTranscriber.Instance.Transcribe(node._primitiveColPath_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._resourcePath._primitiveFunctionImportCall_꘡primitivePath꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._primitiveFunctionImportCallTranscriber.Instance.Transcribe(node._primitiveFunctionImportCall_1, context);
if (node._primitivePath_1 != null)
{
__GeneratedOdataV3.Trancsribers.Rules._primitivePathTranscriber.Instance.Transcribe(node._primitivePath_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._resourcePath._functionImportCallNoParens node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._functionImportCallNoParensTranscriber.Instance.Transcribe(node._functionImportCallNoParens_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._resourcePath._crossjoin node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._crossjoinTranscriber.Instance.Transcribe(node._crossjoin_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._resourcePath._ʺx24x61x6Cx6Cʺ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx24x61x6Cx6CʺTranscriber.Instance.Transcribe(node._ʺx24x61x6Cx6Cʺ_1, context);
if (node._ʺx2Fʺ_qualifiedEntityTypeName_1 != null)
{
__GeneratedOdataV3.Trancsribers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameTranscriber.Instance.Transcribe(node._ʺx2Fʺ_qualifiedEntityTypeName_1, context);
}

return default;
            }
        }
    }
    
}
