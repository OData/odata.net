namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _boundFunctionCallNoParensTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._boundFunctionCallNoParens>
    {
        private _boundFunctionCallNoParensTranscriber()
        {
        }
        
        public static _boundFunctionCallNoParensTranscriber Instance { get; } = new _boundFunctionCallNoParensTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._boundFunctionCallNoParens value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Rules._boundFunctionCallNoParens.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_entityFunction node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(node._namespace_1, context);
__GeneratedOdataV4.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(node._ʺx2Eʺ_1, context);
__GeneratedOdataV4.Trancsribers.Rules._entityFunctionTranscriber.Instance.Transcribe(node._entityFunction_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_entityColFunction node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(node._namespace_1, context);
__GeneratedOdataV4.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(node._ʺx2Eʺ_1, context);
__GeneratedOdataV4.Trancsribers.Rules._entityColFunctionTranscriber.Instance.Transcribe(node._entityColFunction_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_complexFunction node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(node._namespace_1, context);
__GeneratedOdataV4.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(node._ʺx2Eʺ_1, context);
__GeneratedOdataV4.Trancsribers.Rules._complexFunctionTranscriber.Instance.Transcribe(node._complexFunction_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_complexColFunction node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(node._namespace_1, context);
__GeneratedOdataV4.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(node._ʺx2Eʺ_1, context);
__GeneratedOdataV4.Trancsribers.Rules._complexColFunctionTranscriber.Instance.Transcribe(node._complexColFunction_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_primitiveFunction node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(node._namespace_1, context);
__GeneratedOdataV4.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(node._ʺx2Eʺ_1, context);
__GeneratedOdataV4.Trancsribers.Rules._primitiveFunctionTranscriber.Instance.Transcribe(node._primitiveFunction_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_primitiveColFunction node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(node._namespace_1, context);
__GeneratedOdataV4.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(node._ʺx2Eʺ_1, context);
__GeneratedOdataV4.Trancsribers.Rules._primitiveColFunctionTranscriber.Instance.Transcribe(node._primitiveColFunction_1, context);

return default;
            }
        }
    }
    
}
