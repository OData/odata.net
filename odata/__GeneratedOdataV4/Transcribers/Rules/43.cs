namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _functionImportCallNoParensTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens>
    {
        private _functionImportCallNoParensTranscriber()
        {
        }
        
        public static _functionImportCallNoParensTranscriber Instance { get; } = new _functionImportCallNoParensTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._entityFunctionImport node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._entityFunctionImportTranscriber.Instance.Transcribe(node._entityFunctionImport_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._entityColFunctionImport node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._entityColFunctionImportTranscriber.Instance.Transcribe(node._entityColFunctionImport_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._complexFunctionImport node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._complexFunctionImportTranscriber.Instance.Transcribe(node._complexFunctionImport_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._complexColFunctionImport node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._complexColFunctionImportTranscriber.Instance.Transcribe(node._complexColFunctionImport_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._primitiveFunctionImport node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._primitiveFunctionImportTranscriber.Instance.Transcribe(node._primitiveFunctionImport_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._primitiveColFunctionImport node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._primitiveColFunctionImportTranscriber.Instance.Transcribe(node._primitiveColFunctionImport_1, context);

return default;
            }
        }
    }
    
}
