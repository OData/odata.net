namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _selectListItemTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._selectListItem>
    {
        private _selectListItemTranscriber()
        {
        }
        
        public static _selectListItemTranscriber Instance { get; } = new _selectListItemTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._selectListItem value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._selectListItem.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._selectListItem._STAR node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._STARTranscriber.Instance.Transcribe(node._STAR_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._selectListItem._allOperationsInSchema node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._allOperationsInSchemaTranscriber.Instance.Transcribe(node._allOperationsInSchema_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._selectListItem._꘡qualifiedEntityTypeName_ʺx2Fʺ꘡_ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ node, System.Text.StringBuilder context)
            {
                if (node._qualifiedEntityTypeName_ʺx2Fʺ_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._qualifiedEntityTypeName_ʺx2FʺTranscriber.Instance.Transcribe(node._qualifiedEntityTypeName_ʺx2Fʺ_1, context);
}
__GeneratedOdata.Trancsribers.Inners._ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃTranscriber.Instance.Transcribe(node._ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ_1, context);

return default;
            }
        }
    }
    
}
