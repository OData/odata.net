namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _boundActionCallⳆboundEntityColFunctionCall_꘡collectionNavigation꘡ⳆboundEntityFunctionCall_꘡singleNavigation꘡ⳆboundComplexColFunctionCall_꘡complexColPath꘡ⳆboundComplexFunctionCall_꘡complexPath꘡ⳆboundPrimitiveColFunctionCall_꘡primitiveColPath꘡ⳆboundPrimitiveFunctionCall_꘡primitivePath꘡ⳆboundFunctionCallNoParensTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._boundActionCallⳆboundEntityColFunctionCall_꘡collectionNavigation꘡ⳆboundEntityFunctionCall_꘡singleNavigation꘡ⳆboundComplexColFunctionCall_꘡complexColPath꘡ⳆboundComplexFunctionCall_꘡complexPath꘡ⳆboundPrimitiveColFunctionCall_꘡primitiveColPath꘡ⳆboundPrimitiveFunctionCall_꘡primitivePath꘡ⳆboundFunctionCallNoParens>
    {
        private _boundActionCallⳆboundEntityColFunctionCall_꘡collectionNavigation꘡ⳆboundEntityFunctionCall_꘡singleNavigation꘡ⳆboundComplexColFunctionCall_꘡complexColPath꘡ⳆboundComplexFunctionCall_꘡complexPath꘡ⳆboundPrimitiveColFunctionCall_꘡primitiveColPath꘡ⳆboundPrimitiveFunctionCall_꘡primitivePath꘡ⳆboundFunctionCallNoParensTranscriber()
        {
        }
        
        public static _boundActionCallⳆboundEntityColFunctionCall_꘡collectionNavigation꘡ⳆboundEntityFunctionCall_꘡singleNavigation꘡ⳆboundComplexColFunctionCall_꘡complexColPath꘡ⳆboundComplexFunctionCall_꘡complexPath꘡ⳆboundPrimitiveColFunctionCall_꘡primitiveColPath꘡ⳆboundPrimitiveFunctionCall_꘡primitivePath꘡ⳆboundFunctionCallNoParensTranscriber Instance { get; } = new _boundActionCallⳆboundEntityColFunctionCall_꘡collectionNavigation꘡ⳆboundEntityFunctionCall_꘡singleNavigation꘡ⳆboundComplexColFunctionCall_꘡complexColPath꘡ⳆboundComplexFunctionCall_꘡complexPath꘡ⳆboundPrimitiveColFunctionCall_꘡primitiveColPath꘡ⳆboundPrimitiveFunctionCall_꘡primitivePath꘡ⳆboundFunctionCallNoParensTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._boundActionCallⳆboundEntityColFunctionCall_꘡collectionNavigation꘡ⳆboundEntityFunctionCall_꘡singleNavigation꘡ⳆboundComplexColFunctionCall_꘡complexColPath꘡ⳆboundComplexFunctionCall_꘡complexPath꘡ⳆboundPrimitiveColFunctionCall_꘡primitiveColPath꘡ⳆboundPrimitiveFunctionCall_꘡primitivePath꘡ⳆboundFunctionCallNoParens value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Inners._boundActionCallⳆboundEntityColFunctionCall_꘡collectionNavigation꘡ⳆboundEntityFunctionCall_꘡singleNavigation꘡ⳆboundComplexColFunctionCall_꘡complexColPath꘡ⳆboundComplexFunctionCall_꘡complexPath꘡ⳆboundPrimitiveColFunctionCall_꘡primitiveColPath꘡ⳆboundPrimitiveFunctionCall_꘡primitivePath꘡ⳆboundFunctionCallNoParens.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._boundActionCallⳆboundEntityColFunctionCall_꘡collectionNavigation꘡ⳆboundEntityFunctionCall_꘡singleNavigation꘡ⳆboundComplexColFunctionCall_꘡complexColPath꘡ⳆboundComplexFunctionCall_꘡complexPath꘡ⳆboundPrimitiveColFunctionCall_꘡primitiveColPath꘡ⳆboundPrimitiveFunctionCall_꘡primitivePath꘡ⳆboundFunctionCallNoParens._boundActionCall node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._boundActionCallTranscriber.Instance.Transcribe(node._boundActionCall_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._boundActionCallⳆboundEntityColFunctionCall_꘡collectionNavigation꘡ⳆboundEntityFunctionCall_꘡singleNavigation꘡ⳆboundComplexColFunctionCall_꘡complexColPath꘡ⳆboundComplexFunctionCall_꘡complexPath꘡ⳆboundPrimitiveColFunctionCall_꘡primitiveColPath꘡ⳆboundPrimitiveFunctionCall_꘡primitivePath꘡ⳆboundFunctionCallNoParens._boundEntityColFunctionCall_꘡collectionNavigation꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._boundEntityColFunctionCallTranscriber.Instance.Transcribe(node._boundEntityColFunctionCall_1, context);
if (node._collectionNavigation_1 != null)
{
__GeneratedOdataV4.Trancsribers.Rules._collectionNavigationTranscriber.Instance.Transcribe(node._collectionNavigation_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._boundActionCallⳆboundEntityColFunctionCall_꘡collectionNavigation꘡ⳆboundEntityFunctionCall_꘡singleNavigation꘡ⳆboundComplexColFunctionCall_꘡complexColPath꘡ⳆboundComplexFunctionCall_꘡complexPath꘡ⳆboundPrimitiveColFunctionCall_꘡primitiveColPath꘡ⳆboundPrimitiveFunctionCall_꘡primitivePath꘡ⳆboundFunctionCallNoParens._boundEntityFunctionCall_꘡singleNavigation꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._boundEntityFunctionCallTranscriber.Instance.Transcribe(node._boundEntityFunctionCall_1, context);
if (node._singleNavigation_1 != null)
{
__GeneratedOdataV4.Trancsribers.Rules._singleNavigationTranscriber.Instance.Transcribe(node._singleNavigation_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._boundActionCallⳆboundEntityColFunctionCall_꘡collectionNavigation꘡ⳆboundEntityFunctionCall_꘡singleNavigation꘡ⳆboundComplexColFunctionCall_꘡complexColPath꘡ⳆboundComplexFunctionCall_꘡complexPath꘡ⳆboundPrimitiveColFunctionCall_꘡primitiveColPath꘡ⳆboundPrimitiveFunctionCall_꘡primitivePath꘡ⳆboundFunctionCallNoParens._boundComplexColFunctionCall_꘡complexColPath꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._boundComplexColFunctionCallTranscriber.Instance.Transcribe(node._boundComplexColFunctionCall_1, context);
if (node._complexColPath_1 != null)
{
__GeneratedOdataV4.Trancsribers.Rules._complexColPathTranscriber.Instance.Transcribe(node._complexColPath_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._boundActionCallⳆboundEntityColFunctionCall_꘡collectionNavigation꘡ⳆboundEntityFunctionCall_꘡singleNavigation꘡ⳆboundComplexColFunctionCall_꘡complexColPath꘡ⳆboundComplexFunctionCall_꘡complexPath꘡ⳆboundPrimitiveColFunctionCall_꘡primitiveColPath꘡ⳆboundPrimitiveFunctionCall_꘡primitivePath꘡ⳆboundFunctionCallNoParens._boundComplexFunctionCall_꘡complexPath꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._boundComplexFunctionCallTranscriber.Instance.Transcribe(node._boundComplexFunctionCall_1, context);
if (node._complexPath_1 != null)
{
__GeneratedOdataV4.Trancsribers.Rules._complexPathTranscriber.Instance.Transcribe(node._complexPath_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._boundActionCallⳆboundEntityColFunctionCall_꘡collectionNavigation꘡ⳆboundEntityFunctionCall_꘡singleNavigation꘡ⳆboundComplexColFunctionCall_꘡complexColPath꘡ⳆboundComplexFunctionCall_꘡complexPath꘡ⳆboundPrimitiveColFunctionCall_꘡primitiveColPath꘡ⳆboundPrimitiveFunctionCall_꘡primitivePath꘡ⳆboundFunctionCallNoParens._boundPrimitiveColFunctionCall_꘡primitiveColPath꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._boundPrimitiveColFunctionCallTranscriber.Instance.Transcribe(node._boundPrimitiveColFunctionCall_1, context);
if (node._primitiveColPath_1 != null)
{
__GeneratedOdataV4.Trancsribers.Rules._primitiveColPathTranscriber.Instance.Transcribe(node._primitiveColPath_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._boundActionCallⳆboundEntityColFunctionCall_꘡collectionNavigation꘡ⳆboundEntityFunctionCall_꘡singleNavigation꘡ⳆboundComplexColFunctionCall_꘡complexColPath꘡ⳆboundComplexFunctionCall_꘡complexPath꘡ⳆboundPrimitiveColFunctionCall_꘡primitiveColPath꘡ⳆboundPrimitiveFunctionCall_꘡primitivePath꘡ⳆboundFunctionCallNoParens._boundPrimitiveFunctionCall_꘡primitivePath꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._boundPrimitiveFunctionCallTranscriber.Instance.Transcribe(node._boundPrimitiveFunctionCall_1, context);
if (node._primitivePath_1 != null)
{
__GeneratedOdataV4.Trancsribers.Rules._primitivePathTranscriber.Instance.Transcribe(node._primitivePath_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._boundActionCallⳆboundEntityColFunctionCall_꘡collectionNavigation꘡ⳆboundEntityFunctionCall_꘡singleNavigation꘡ⳆboundComplexColFunctionCall_꘡complexColPath꘡ⳆboundComplexFunctionCall_꘡complexPath꘡ⳆboundPrimitiveColFunctionCall_꘡primitiveColPath꘡ⳆboundPrimitiveFunctionCall_꘡primitivePath꘡ⳆboundFunctionCallNoParens._boundFunctionCallNoParens node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._boundFunctionCallNoParensTranscriber.Instance.Transcribe(node._boundFunctionCallNoParens_1, context);

return default;
            }
        }
    }
    
}
