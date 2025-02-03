namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _selectItemTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._selectItem>
    {
        private _selectItemTranscriber()
        {
        }
        
        public static _selectItemTranscriber Instance { get; } = new _selectItemTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._selectItem value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Rules._selectItem.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._selectItem._STAR node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._STARTranscriber.Instance.Transcribe(node._STAR_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._selectItem._allOperationsInSchema node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._allOperationsInSchemaTranscriber.Instance.Transcribe(node._allOperationsInSchema_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._selectItem._꘡ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ꘡_ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃ node, System.Text.StringBuilder context)
            {
                if (node._ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ_1 != null)
{
__GeneratedOdataV3.Trancsribers.Inners._ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2FʺTranscriber.Instance.Transcribe(node._ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ_1, context);
}
__GeneratedOdataV3.Trancsribers.Inners._ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃTranscriber.Instance.Transcribe(node._ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃ_1, context);

return default;
            }
        }
    }
    
}
