namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _functionImportCallNoParensTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._functionImportCallNoParens>
    {
        private _functionImportCallNoParensTranscriber()
        {
        }
        
        public static _functionImportCallNoParensTranscriber Instance { get; } = new _functionImportCallNoParensTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._functionImportCallNoParens value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._functionImportCallNoParens.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._functionImportCallNoParens._entityFunctionImport node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._entityFunctionImportTranscriber.Instance.Transcribe(node._entityFunctionImport_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._functionImportCallNoParens._entityColFunctionImport node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._entityColFunctionImportTranscriber.Instance.Transcribe(node._entityColFunctionImport_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._functionImportCallNoParens._complexFunctionImport node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._complexFunctionImportTranscriber.Instance.Transcribe(node._complexFunctionImport_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._functionImportCallNoParens._complexColFunctionImport node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._complexColFunctionImportTranscriber.Instance.Transcribe(node._complexColFunctionImport_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._functionImportCallNoParens._primitiveFunctionImport node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._primitiveFunctionImportTranscriber.Instance.Transcribe(node._primitiveFunctionImport_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._functionImportCallNoParens._primitiveColFunctionImport node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._primitiveColFunctionImportTranscriber.Instance.Transcribe(node._primitiveColFunctionImport_1, context);

return default;
            }
        }
    }
    
}
