namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _propertyPathTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._propertyPath>
    {
        private _propertyPathTranscriber()
        {
        }
        
        public static _propertyPathTranscriber Instance { get; } = new _propertyPathTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._propertyPath value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Rules._propertyPath.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._propertyPath._entityColNavigationProperty_꘡collectionNavigation꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._entityColNavigationPropertyTranscriber.Instance.Transcribe(node._entityColNavigationProperty_1, context);
if (node._collectionNavigation_1 != null)
{
__GeneratedOdataV4.Trancsribers.Rules._collectionNavigationTranscriber.Instance.Transcribe(node._collectionNavigation_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._propertyPath._entityNavigationProperty_꘡singleNavigation꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._entityNavigationPropertyTranscriber.Instance.Transcribe(node._entityNavigationProperty_1, context);
if (node._singleNavigation_1 != null)
{
__GeneratedOdataV4.Trancsribers.Rules._singleNavigationTranscriber.Instance.Transcribe(node._singleNavigation_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._propertyPath._complexColProperty_꘡complexColPath꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._complexColPropertyTranscriber.Instance.Transcribe(node._complexColProperty_1, context);
if (node._complexColPath_1 != null)
{
__GeneratedOdataV4.Trancsribers.Rules._complexColPathTranscriber.Instance.Transcribe(node._complexColPath_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._propertyPath._complexProperty_꘡complexPath꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._complexPropertyTranscriber.Instance.Transcribe(node._complexProperty_1, context);
if (node._complexPath_1 != null)
{
__GeneratedOdataV4.Trancsribers.Rules._complexPathTranscriber.Instance.Transcribe(node._complexPath_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._propertyPath._primitiveColProperty_꘡primitiveColPath꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._primitiveColPropertyTranscriber.Instance.Transcribe(node._primitiveColProperty_1, context);
if (node._primitiveColPath_1 != null)
{
__GeneratedOdataV4.Trancsribers.Rules._primitiveColPathTranscriber.Instance.Transcribe(node._primitiveColPath_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._propertyPath._primitiveProperty_꘡primitivePath꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._primitivePropertyTranscriber.Instance.Transcribe(node._primitiveProperty_1, context);
if (node._primitivePath_1 != null)
{
__GeneratedOdataV4.Trancsribers.Rules._primitivePathTranscriber.Instance.Transcribe(node._primitivePath_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._propertyPath._streamProperty_꘡boundOperation꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._streamPropertyTranscriber.Instance.Transcribe(node._streamProperty_1, context);
if (node._boundOperation_1 != null)
{
__GeneratedOdataV4.Trancsribers.Rules._boundOperationTranscriber.Instance.Transcribe(node._boundOperation_1, context);
}

return default;
            }
        }
    }
    
}
